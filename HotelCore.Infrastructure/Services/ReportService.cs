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
    public class ReportService : IReportService
    {
        private readonly string _connectionString;

        public ReportService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<AnalyticsOverviewDto> GetAnalyticsOverviewAsync(DateTime startDate, DateTime endDate)
        {
            using var db = CreateConnection();
            
            // 1. Tổng quan các chỉ số
            string overviewSql = @"
                SELECT 
                    (SELECT ISNULL(SUM(TotalAmount), 0) FROM Invoices WHERE InvoiceDate BETWEEN @Start AND @End) as TotalRevenue,
                    (SELECT COUNT(*) FROM Bookings WHERE BookingDate BETWEEN @Start AND @End) as TotalBookings,
                    (SELECT COUNT(*) FROM Customers) as TotalCustomers,
                    (CAST((SELECT COUNT(*) FROM Rooms WHERE Status = 'Occupied') AS FLOAT) / 
                     CAST((SELECT COUNT(*) FROM Rooms) AS FLOAT)) * 100 as OccupancyRate";

            var overview = await db.QueryFirstOrDefaultAsync<dynamic>(overviewSql, new { Start = startDate, End = endDate });

            // 2. Doanh thu theo ngày (Cho biểu đồ miền)
            string revenueSql = @"
                SELECT 
                    FORMAT(InvoiceDate, 'dd/MM') as DateLabel,
                    SUM(TotalAmount) as Revenue,
                    COUNT(*) as BookingCount
                FROM Invoices
                WHERE InvoiceDate BETWEEN @Start AND @End
                GROUP BY FORMAT(InvoiceDate, 'dd/MM'), CAST(InvoiceDate AS DATE)
                ORDER BY CAST(InvoiceDate AS DATE)";
            
            var revenueData = await db.QueryAsync<RevenueDataDto>(revenueSql, new { Start = startDate, End = endDate });

            // 3. Trạng thái phòng (Cho biểu đồ tròn)
            string statusSql = "SELECT Status, COUNT(*) as Count FROM Rooms GROUP BY Status";
            var statusData = await db.QueryAsync<RoomStatusStatDto>(statusSql);

            // 4. Top Dịch vụ
            string topServicesSql = @"
                SELECT TOP 5
                    s.ServiceName,
                    SUM(bs.Quantity) as UsageCount,
                    SUM(bs.Quantity * bs.Price) as TotalRevenue
                FROM BookingServices bs
                JOIN Services s ON bs.ServiceId = s.ServiceId
                GROUP BY s.ServiceName
                ORDER BY TotalRevenue DESC";
            
            var topServices = await db.QueryAsync<TopServiceDto>(topServicesSql);

            return new AnalyticsOverviewDto
            {
                TotalRevenue = overview?.TotalRevenue ?? 0,
                TotalBookings = overview?.TotalBookings ?? 0,
                TotalCustomers = overview?.TotalCustomers ?? 0,
                OccupancyRate = Math.Round((double)(overview?.OccupancyRate ?? 0), 1),
                RevenueByDay = revenueData.ToList(),
                RoomStatusStats = statusData.ToList(),
                TopServices = topServices.ToList()
            };
        }

        public async Task<AnalyticsOverviewDto> GetYearlyAnalyticsAsync(int year)
        {
            using var db = CreateConnection();
            
            string revenueSql = @"
                SELECT 
                    FORMAT(InvoiceDate, 'MM/yyyy') as DateLabel,
                    SUM(TotalAmount) as Revenue,
                    COUNT(*) as BookingCount
                FROM Invoices
                WHERE YEAR(InvoiceDate) = @Year
                GROUP BY FORMAT(InvoiceDate, 'MM/yyyy'), MONTH(InvoiceDate)
                ORDER BY MONTH(InvoiceDate)";
            
            var revenueData = await db.QueryAsync<RevenueDataDto>(revenueSql, new { Year = year });

            return new AnalyticsOverviewDto
            {
                RevenueByDay = revenueData.ToList()
            };
        }
    }
}
