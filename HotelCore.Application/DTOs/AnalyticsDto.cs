using System;
using System.Collections.Generic;

namespace HotelCore.Application.DTOs
{
    public class AnalyticsOverviewDto
    {
        // Thống kê dạng số (Cards)
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TotalCustomers { get; set; }
        public double OccupancyRate { get; set; }

        // Dữ liệu biểu đồ doanh thu (Line/Area Chart)
        public List<RevenueDataDto> RevenueByDay { get; set; }

        // Dữ liệu biểu đồ trạng thái phòng (Doughnut Chart)
        public List<RoomStatusStatDto> RoomStatusStats { get; set; }

        // Top Dịch vụ
        public List<TopServiceDto> TopServices { get; set; }
    }

    public class RevenueDataDto
    {
        public string DateLabel { get; set; }
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
    }

    public class RoomStatusStatDto
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class TopServiceDto
    {
        public string ServiceName { get; set; }
        public int UsageCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
