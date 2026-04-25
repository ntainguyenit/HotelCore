using System;
using System.Collections.Generic;

namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) chứa toàn bộ dữ liệu thống kê tổng quan 
    /// hiển thị trên trang Dashboard của hệ thống quản lý Khách Sạn.
    /// </summary>
    public class DashboardOverviewDto
    {
        /// <summary>
        /// Tổng số phòng hiện có trong khách sạn.
        /// </summary>
        public int TotalRooms { get; set; }

        /// <summary>
        /// Số lượng phòng đang trống (Available).
        /// </summary>
        public int AvailableRooms { get; set; }

        /// <summary>
        /// Số lượng phòng đang có khách thuê (Occupied).
        /// </summary>
        public int OccupiedRooms { get; set; }

        /// <summary>
        /// Số lượng khách sẽ Check-in trong ngày hôm nay.
        /// </summary>
        public int CheckInToday { get; set; }

        /// <summary>
        /// Tổng doanh thu của ngày hôm nay (tính từ 00:00).
        /// </summary>
        public decimal TodayRevenue { get; set; }

        /// <summary>
        /// Danh sách chi tiết các phòng để hiển thị lên lưới sơ đồ phòng (Room Grid).
        /// </summary>
        public List<RoomGridDto> RoomGrids { get; set; } = new List<RoomGridDto>();
    }
}
