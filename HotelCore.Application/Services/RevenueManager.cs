using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelCore.Application.Interfaces;
using HotelCore.Domain.Entities;

namespace HotelCore.Application.Services
{
    /// <summary>
    /// Triển khai dịch vụ phân tích tài chính doanh thu khách sạn nâng cao.
    /// Cung cấp các chỉ số vận hành quan trọng như ADR, RevPAR, Occupancy Rate và Dự báo xu hướng.
    /// </summary>
    public class RevenueManager : IRevenueManager
    {
        public decimal CalculateOccupancyRate(int totalRooms, int occupiedRooms)
        {
            if (totalRooms <= 0) return 0;
            if (occupiedRooms < 0) return 0;

            var rate = (decimal)occupiedRooms / totalRooms;
            return Math.Round(rate, 4); // Trả về dạng tỷ lệ vd: 0.7552 (75.52%)
        }

        public decimal CalculateAverageDailyRate(decimal totalRoomRevenue, int occupiedRoomsCount)
        {
            if (occupiedRoomsCount <= 0) return 0;
            if (totalRoomRevenue < 0) return 0;

            var adr = totalRoomRevenue / occupiedRoomsCount;
            return Math.Round(adr, 2);
        }

        public decimal CalculateRevPar(decimal totalRoomRevenue, int totalRoomsCount)
        {
            if (totalRoomsCount <= 0) return 0;
            if (totalRoomRevenue < 0) return 0;

            var revPar = totalRoomRevenue / totalRoomsCount;
            return Math.Round(revPar, 2);
        }

        public async Task<Dictionary<string, decimal>> ForecastOccupancyTrendAsync(
            IEnumerable<Booking> futureBookings, 
            int totalRoomsCount, 
            int daysToForecast)
        {
            if (daysToForecast <= 0) daysToForecast = 7;
            if (totalRoomsCount <= 0) totalRoomsCount = 1; // Tránh chia cho 0

            var forecast = new Dictionary<string, decimal>();
            var today = DateTime.Today;

            // Chạy vòng lặp dự báo cho N ngày tiếp theo kể từ hôm nay
            for (int i = 0; i < daysToForecast; i++)
            {
                var forecastDate = today.AddDays(i);
                
                // Lấy số lượng phòng dự kiến sẽ có khách lưu trú trong ngày forecastDate
                int occupiedRooms = futureBookings.Count(b => 
                    b.DateRange != null && 
                    b.DateRange.Contains(forecastDate) && 
                    b.Status != Domain.Enums.BookingStatus.Cancelled);

                decimal rate = CalculateOccupancyRate(totalRoomsCount, occupiedRooms);
                
                // Lưu vào kết quả dự báo
                string dateKey = forecastDate.ToString("yyyy-MM-dd");
                forecast[dateKey] = rate;
            }

            return await Task.FromResult(forecast);
        }
    }
}
