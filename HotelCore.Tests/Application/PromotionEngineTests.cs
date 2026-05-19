using System;
using System.Threading.Tasks;
using HotelCore.Application.Services;
using HotelCore.Domain.Entities;
using HotelCore.Domain.Enums;
using HotelCore.Domain.ValueObjects;
using Xunit;

namespace HotelCore.Tests.Application
{
    /// <summary>
    /// Các bài kiểm thử đơn vị cho bộ tính toán khuyến mãi PromotionEngine.
    /// </summary>
    public class PromotionEngineTests
    {
        private readonly PromotionEngine _engine;

        public PromotionEngineTests()
        {
            _engine = new PromotionEngine();
        }

        [Fact]
        public async Task CalculateDiscountRate_NoCustomerAndNoPromo_ShortStay_ShouldReturnZeroDiscount()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2)); // 2 ngày (không đạt mốc giảm giá ngày ở)
            var booking = new Booking(1, 101, range, 500000m);
            // Không gán Customer

            // Act
            var rate = await _engine.CalculateDiscountRateAsync(booking, null);

            // Assert
            Assert.Equal(0.00m, rate);
        }

        [Fact]
        public async Task CalculateDiscountRate_LongStayDiscount_ShouldApplyRate()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(6)); // 6 ngày -> giảm 8%
            var booking = new Booking(1, 101, range, 500000m);
            // Không gán Customer

            // Act
            var rate = await _engine.CalculateDiscountRateAsync(booking, null);

            // Assert
            Assert.Equal(0.08m, rate); // Giảm 8%
        }

        [Fact]
        public async Task CalculateDiscountRate_CustomerLoyaltyTier_ShouldCombineWithStayDuration()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(6)); // Stay: 6 ngày -> +8%
            var customer = new Customer("John Doe", "john@test.com", "0987654321", "123456");
            customer.AddPoints(6000); // 6000 points -> Hạng Gold -> +6%
            
            var booking = new Booking(1, 101, range, 500000m)
            {
                Customer = customer
            };

            // Act
            var rate = await _engine.CalculateDiscountRateAsync(booking, null);

            // Assert
            // Mong đợi: 8% (Stay duration) + 6% (Gold tier) = 14% -> 0.14
            Assert.Equal(0.14m, rate);
        }

        [Fact]
        public async Task CalculateDiscountRate_PromoCode_ShouldApplyAdditionalDiscount()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2)); // Stay: 2 ngày -> +0%
            var customer = new Customer("John Doe", "john@test.com", "0987654321", "123456"); // Silver -> +1%
            
            var booking = new Booking(1, 101, range, 500000m)
            {
                Customer = customer
            };

            // Act
            var rate = await _engine.CalculateDiscountRateAsync(booking, "SUMMERVIBES"); // +20%

            // Assert
            // Mong đợi: 0% + 1% + 20% = 21% -> 0.21
            Assert.Equal(0.21m, rate);
        }

        [Fact]
        public async Task CalculateDiscountRate_ExtremelyHighDiscount_ShouldBeCapped()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(12)); // Stay: 12 ngày -> +15%
            var customer = new Customer("Platinum VIP", "vip@test.com", "0987654321", "123456");
            customer.AddPoints(20000); // Platinum -> +12%
            
            var booking = new Booking(1, 101, range, 1000000m)
            {
                Customer = customer
            };

            // Act
            var rate = await _engine.CalculateDiscountRateAsync(booking, "SUMMERVIBES"); // +20%
            // Tổng lý thuyết: 15% + 12% + 20% = 47%

            // Assert
            // Phải bị giới hạn tối đa (Capped) ở mức 35% -> 0.35
            Assert.Equal(0.35m, rate);
        }
    }
}
