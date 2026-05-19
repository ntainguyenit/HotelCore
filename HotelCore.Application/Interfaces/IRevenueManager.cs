using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Domain.Entities;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện xử lý tính toán các báo cáo doanh thu tài chính chuyên sâu (ADR, Occupancy Rate, RevPAR).
    /// </summary>
    public interface IRevenueManager
    {
        /// <summary>
        /// Tính tỷ lệ lấp đầy phòng (Occupancy Rate).
        /// </summary>
        decimal CalculateOccupancyRate(int totalRooms, int occupiedRooms);

        /// <summary>
        /// Tính giá phòng trung bình bán được trong ngày (Average Daily Rate - ADR).
        /// </summary>
        decimal CalculateAverageDailyRate(decimal totalRoomRevenue, int occupiedRoomsCount);

        /// <summary>
        /// Tính doanh thu trên mỗi phòng có sẵn (Revenue Per Available Room - RevPAR).
        /// </summary>
        decimal CalculateRevPar(decimal totalRoomRevenue, int totalRoomsCount);

        /// <summary>
        /// Dự báo xu hướng công suất phòng trong tương lai gần dựa trên danh sách lịch đặt trước.
        /// </summary>
        Task<Dictionary<string, decimal>> ForecastOccupancyTrendAsync(IEnumerable<Booking> futureBookings, int totalRoomsCount, int daysToForecast);
    }
}
