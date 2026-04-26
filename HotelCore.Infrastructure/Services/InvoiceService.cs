using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelCore.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly string _connectionString;

        public InvoiceService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is not configured.");
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                SELECT i.InvoiceId, c.FullName AS CustomerName, r.RoomNumber, i.InvoiceDate, 
                       i.RoomTotal as RoomAmount, i.ServiceTotal as ServiceAmount, i.TaxAmount, 
                       i.TotalAmount, i.PaymentMethod
                FROM Invoices i
                JOIN Bookings b ON i.BookingId = b.BookingId
                JOIN Customers c ON b.CustomerId = c.CustomerId
                JOIN BookingRooms br ON b.BookingId = br.BookingId
                JOIN Rooms r ON br.RoomId = r.RoomId
                ORDER BY i.InvoiceDate DESC";
            return await db.QueryAsync<InvoiceDto>(sql);
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                SELECT i.InvoiceId, c.FullName AS CustomerName, r.RoomNumber, i.InvoiceDate, 
                       i.RoomTotal as RoomAmount, i.ServiceTotal as ServiceAmount, i.TaxAmount, 
                       i.TotalAmount, i.PaymentMethod
                FROM Invoices i
                JOIN Bookings b ON i.BookingId = b.BookingId
                JOIN Customers c ON b.CustomerId = c.CustomerId
                JOIN BookingRooms br ON b.BookingId = br.BookingId
                JOIN Rooms r ON br.RoomId = r.RoomId
                WHERE i.InvoiceId = @Id";
            return await db.QueryFirstOrDefaultAsync<InvoiceDto>(sql, new { Id = id });
        }

        public async Task<CheckoutViewDto?> GetBookingForCheckoutAsync(int bookingId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            
            // 1. Lấy thông tin cơ bản của đặt phòng
            string bookingSql = @"
                SELECT b.BookingId, c.FullName AS CustomerName, r.RoomNumber, rt.TypeName AS RoomType, 
                       b.CheckInDate, b.CheckOutDate, br.Price AS RoomPricePerDay
                FROM Bookings b
                JOIN Customers c ON b.CustomerId = c.CustomerId
                JOIN BookingRooms br ON b.BookingId = br.BookingId
                JOIN Rooms r ON br.RoomId = r.RoomId
                JOIN RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
                WHERE b.BookingId = @BookingId";

            var checkout = await db.QuerySingleOrDefaultAsync<CheckoutViewDto>(bookingSql, new { BookingId = bookingId });
            if (checkout == null) return null;

            // Tính số ngày và tiền phòng
            checkout.TotalDays = (int)(checkout.CheckOutDate - checkout.CheckInDate).TotalDays;
            if (checkout.TotalDays == 0) checkout.TotalDays = 1; // Tính tối thiểu 1 ngày
            checkout.RoomTotal = checkout.TotalDays * checkout.RoomPricePerDay;

            // 2. Lấy thông tin dịch vụ đã sử dụng
            string servicesSql = @"
                SELECT s.ServiceName, bs.Quantity, bs.Price
                FROM BookingServices bs
                JOIN Services s ON bs.ServiceId = s.ServiceId
                WHERE bs.BookingId = @BookingId";
            
            var services = await db.QueryAsync<InvoiceServiceDetailDto>(servicesSql, new { BookingId = bookingId });
            checkout.Services = services.ToList();
            checkout.ServiceTotal = checkout.Services.Sum(s => s.SubTotal);

            // 3. Tính thuế (ví dụ 10%) và tổng cộng
            checkout.TaxAmount = (checkout.RoomTotal + checkout.ServiceTotal) * 0.1m;
            checkout.TotalAmount = checkout.RoomTotal + checkout.ServiceTotal + checkout.TaxAmount;

            return checkout;
        }

        public async Task<bool> CreateInvoiceAsync(InvoiceCreateDto invoice)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            db.Open();
            using var transaction = db.BeginTransaction();
            try
            {
                // 1. Tạo hóa đơn
                string invoiceSql = @"
                    INSERT INTO Invoices (BookingId, EmployeeId, InvoiceDate, RoomTotal, ServiceTotal, TaxAmount, TotalAmount, PaymentMethod)
                    VALUES (@BookingId, 1, GETDATE(), @RoomTotal, @ServiceTotal, @TaxAmount, @TotalAmount, @PaymentMethod)";
                // Tạm thời EmployeeId = 1
                await db.ExecuteAsync(invoiceSql, invoice, transaction);

                // 2. Cập nhật trạng thái đặt phòng
                string bookingSql = "UPDATE Bookings SET Status = 'CheckedOut', TotalAmount = @TotalAmount WHERE BookingId = @BookingId";
                await db.ExecuteAsync(bookingSql, new { invoice.TotalAmount, invoice.BookingId }, transaction);

                // 3. Cập nhật trạng thái phòng sang 'Cleaning'
                string roomSql = @"
                    UPDATE Rooms SET Status = 'Cleaning' 
                    WHERE RoomId IN (SELECT RoomId FROM BookingRooms WHERE BookingId = @BookingId)";
                await db.ExecuteAsync(roomSql, new { invoice.BookingId }, transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
