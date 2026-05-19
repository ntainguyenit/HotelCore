using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Application.Services;
using HotelCore.Domain.Entities;
using HotelCore.Domain.Enums;
using HotelCore.Domain.ValueObjects;
using Xunit;

namespace HotelCore.Tests.Application
{
    /// <summary>
    /// Các bài kiểm thử đơn vị cho dịch vụ RevenueManager.
    /// </summary>
    public class RevenueManagerTests
    {
        private readonly RevenueManager _revenueManager;

        public RevenueManagerTests()
        {
            _revenueManager = new RevenueManager();
        }

        [Fact]
        public void CalculateOccupancyRate_ShouldReturnExpectedRatio()
        {
            // Act
            var rate = _revenueManager.CalculateOccupancyRate(100, 75);

            // Assert
            Assert.Equal(0.7500m, rate); // 75%
        }

        [Fact]
        public void CalculateAverageDailyRate_ShouldReturnExpectedADR()
        {
            // Act
            var adr = _revenueManager.CalculateAverageDailyRate(15000000m, 10);

            // Assert
            Assert.Equal(1500000m, adr); // 1.5M/phòng
        }

        [Fact]
        public void CalculateRevPar_ShouldReturnExpectedRevPar()
        {
            // Act
            var revPar = _revenueManager.CalculateRevPar(15000000m, 20);

            // Assert
            // Doanh thu 15M / Tổng 20 phòng = 750K
            Assert.Equal(750000m, revPar);
        }

        [Fact]
        public async Task ForecastOccupancyTrend_ShouldPredictCorrectly()
        {
            // Arrange
            int totalRooms = 10;
            var today = DateTime.Today;

            // Tạo các mock booking lịch làm việc chồng lấn
            var b1 = new Booking(1, 101, new DateRange(today, today.AddDays(3)), 500000m); // Ngày 0, 1, 2
            var b2 = new Booking(2, 102, new DateRange(today.AddDays(1), today.AddDays(4)), 500000m); // Ngày 1, 2, 3
            var b3 = new Booking(3, 103, new DateRange(today.AddDays(2), today.AddDays(3)), 500000m); // Ngày 2

            // Hủy b3 (không được tính vào công suất phòng)
            b3.Cancel();

            var bookings = new List<Booking> { b1, b2, b3 };

            // Act
            var forecast = await _revenueManager.ForecastOccupancyTrendAsync(bookings, totalRooms, 5);

            // Assert
            // Phân tích:
            // Ngày 0 (hôm nay): Chỉ b1 lưu trú => 1 phòng/10 = 10% (0.10)
            // Ngày 1 (ngày mai): b1 và b2 lưu trú => 2 phòng/10 = 20% (0.20)
            // Ngày 2 (ngày kia): b1 và b2 lưu trú (b3 bị hủy) => 2 phòng/10 = 20% (0.20)
            // Ngày 3 (kế tiếp): b2 lưu trú (b1 đã checkout) => 1 phòng/10 = 10% (0.10)
            // Ngày 4 (kế tiếp): không ai ở => 0 phòng = 0% (0.00)

            Assert.Equal(5, forecast.Count);
            Assert.Equal(0.1000m, forecast[today.ToString("yyyy-MM-dd")]);
            Assert.Equal(0.2000m, forecast[today.AddDays(1).ToString("yyyy-MM-dd")]);
            Assert.Equal(0.2000m, forecast[today.AddDays(2).ToString("yyyy-MM-dd")]);
            Assert.Equal(0.1000m, forecast[today.AddDays(3).ToString("yyyy-MM-dd")]);
            Assert.Equal(0.0000m, forecast[today.AddDays(4).ToString("yyyy-MM-dd")]);
        }
    }
}
