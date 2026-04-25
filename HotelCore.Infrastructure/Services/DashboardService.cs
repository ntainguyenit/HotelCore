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
    /// <summary>
    /// Triển khai dịch vụ Dashboard sử dụng thư viện Dapper để tương tác với cơ sở dữ liệu.
    /// Tối ưu hóa hiệu suất truy vấn SQL Server.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo DashboardService với Dependency Injection cho IConfiguration.
        /// </summary>
        /// <param name="configuration">Cấu hình chứa chuỗi kết nối Database.</param>
        public DashboardService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("Connection string is not configured.");
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu thống kê tổng quan và danh sách lưới phòng.
        /// </summary>
        /// <returns>Đối tượng DashboardOverviewDto chứa toàn bộ số liệu.</returns>
        public async Task<DashboardOverviewDto> GetDashboardOverviewAsync()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var result = new DashboardOverviewDto();

            // Truy vấn 1: Lấy số liệu thống kê tổng quan
            string statsSql = @"
                SELECT 
                    (SELECT COUNT(*) FROM Rooms) AS TotalRooms,
                    (SELECT COUNT(*) FROM Rooms WHERE Status = 'Available') AS AvailableRooms,
                    (SELECT COUNT(*) FROM Rooms WHERE Status = 'Occupied') AS OccupiedRooms,
                    (SELECT COUNT(*) FROM Bookings WHERE CONVERT(date, CheckInDate) = CONVERT(date, GETDATE())) AS CheckInToday,
                    ISNULL((SELECT SUM(TotalAmount) FROM Invoices WHERE CONVERT(date, InvoiceDate) = CONVERT(date, GETDATE())), 0) AS TodayRevenue;
            ";

            var stats = await db.QuerySingleOrDefaultAsync<DashboardOverviewDto>(statsSql);
            if (stats != null)
            {
                result.TotalRooms = stats.TotalRooms;
                result.AvailableRooms = stats.AvailableRooms;
                result.OccupiedRooms = stats.OccupiedRooms;
                result.CheckInToday = stats.CheckInToday;
                result.TodayRevenue = stats.TodayRevenue;
            }

            // Truy vấn 2: Lấy danh sách các phòng và khách hàng đang thuê (nếu có)
            string gridSql = @"
                SELECT 
                    r.RoomNumber,
                    r.Floor,
                    rt.TypeName,
                    rt.Capacity,
                    r.Status,
                    c.FullName AS CurrentCustomerName
                FROM Rooms r
                INNER JOIN RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
                LEFT JOIN BookingRooms br ON r.RoomId = br.RoomId
                LEFT JOIN Bookings b ON br.BookingId = b.BookingId AND b.Status = 'CheckedIn'
                LEFT JOIN Customers c ON b.CustomerId = c.CustomerId
                ORDER BY r.Floor, r.RoomNumber;
            ";

            var gridList = await db.QueryAsync<RoomGridDto>(gridSql);
            result.RoomGrids = gridList.ToList();

            return result;
        }
    }
}
