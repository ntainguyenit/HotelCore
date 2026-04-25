using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelCore.Infrastructure.Services
{
    /// <summary>
    /// Triển khai dịch vụ Đặt phòng bằng Dapper.
    /// Xử lý Transaction phức tạp để đảm bảo toàn vẹn dữ liệu khi tạo Booking.
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly string _connectionString;

        public BookingService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is not configured.");
        }

        public async Task<IEnumerable<CustomerDropdownDto>> GetCustomersAsync()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT CustomerId, FullName + ' - ' + Phone AS DisplayName FROM Customers ORDER BY FullName";
            return await db.QueryAsync<CustomerDropdownDto>(sql);
        }

        public async Task<IEnumerable<RoomDropdownDto>> GetAvailableRoomsAsync()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            // Chỉ lấy những phòng có Status = 'Available'
            string sql = @"
                SELECT r.RoomId, 
                       'Phòng ' + r.RoomNumber + ' (' + rt.TypeName + ') - ' + FORMAT(rt.BasePrice, 'N0') + ' đ' AS DisplayName 
                FROM Rooms r
                INNER JOIN RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
                WHERE r.Status = 'Available'
                ORDER BY r.Floor, r.RoomNumber";
            return await db.QueryAsync<RoomDropdownDto>(sql);
        }

        public async Task<bool> CreateBookingAsync(BookingRequestDto request)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            db.Open();

            // Khởi tạo Transaction. Nếu có bất kỳ lỗi nào, mọi thao tác INSERT/UPDATE sẽ bị hủy bỏ.
            using IDbTransaction transaction = db.BeginTransaction();
            try
            {
                // 1. Lấy BasePrice của phòng đang chọn
                string priceSql = "SELECT rt.BasePrice FROM Rooms r INNER JOIN RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId WHERE r.RoomId = @RoomId";
                decimal basePrice = await db.QuerySingleOrDefaultAsync<decimal>(priceSql, new { request.RoomId }, transaction);

                // 2. Thêm mới bản ghi vào bảng Bookings và lấy về BookingId vừa sinh ra
                string insertBookingSql = @"
                    INSERT INTO Bookings (CustomerId, EmployeeId, BookingDate, CheckInDate, CheckOutDate, Status, Notes)
                    VALUES (@CustomerId, 1, GETDATE(), @CheckInDate, @CheckOutDate, 'Confirmed', @Notes);
                    SELECT CAST(SCOPE_IDENTITY() as int);
                ";
                // Lưu ý: EmployeeId tạm thời truyền cứng là 1 (Quản lý) trong bài toán này.
                int newBookingId = await db.QuerySingleAsync<int>(insertBookingSql, new
                {
                    request.CustomerId,
                    request.CheckInDate,
                    request.CheckOutDate,
                    request.Notes
                }, transaction);

                // 3. Thêm chi tiết phòng đặt vào bảng BookingRooms (lưu lại giá tại thời điểm đặt)
                string insertBookingRoomSql = @"
                    INSERT INTO BookingRooms (BookingId, RoomId, Price)
                    VALUES (@BookingId, @RoomId, @Price);
                ";
                await db.ExecuteAsync(insertBookingRoomSql, new
                {
                    BookingId = newBookingId,
                    request.RoomId,
                    Price = basePrice
                }, transaction);

                // 4. Đổi trạng thái phòng thành 'Occupied' (Đang thuê)
                string updateRoomSql = "UPDATE Rooms SET Status = 'Occupied' WHERE RoomId = @RoomId";
                await db.ExecuteAsync(updateRoomSql, new { request.RoomId }, transaction);

                // Nếu mọi thứ chạy mượt mà đến đây, chúng ta COMMIT transaction để lưu vào DB thật.
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Lỗi xảy ra (vd mất kết nối, lỗi logic, v.v.), ROLLBACK hủy toàn bộ dữ liệu đang làm dở.
                transaction.Rollback();
                // Ghi log lỗi ở đây (nếu có ILogger)
                Console.WriteLine($"Error creating booking: {ex.Message}");
                return false;
            }
        }
    }
}
