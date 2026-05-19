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
    /// Các bài kiểm thử đơn vị cho dịch vụ tích lũy điểm LoyaltyService.
    /// </summary>
    public class LoyaltyServiceTests
    {
        private readonly LoyaltyService _loyaltyService;

        public LoyaltyServiceTests()
        {
            _loyaltyService = new LoyaltyService();
        }

        [Fact]
        public async Task CalculatePointsEarned_InvoiceNotPaid_ShouldReturnZeroPoints()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(3));
            var booking = new Booking(1, 101, range, 500000m); // Room charge = 1.5M
            var invoice = new Invoice(booking, 0.0m); // IsPaid = false

            // Act
            var points = await _loyaltyService.CalculatePointsEarnedAsync(invoice);

            // Assert
            Assert.Equal(0, points);
        }

        [Fact]
        public async Task CalculatePointsEarned_PaidInvoice_SilverMember_ShouldCalculateBasePoints()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var customer = new Customer("Guest", "g@t.com", "123", "456"); // Silver
            var booking = new Booking(1, 101, range, 500000m) { Customer = customer }; // Room charge = 1M
            
            // Sử dụng thêm dịch vụ: Spa 150K
            booking.CheckIn();
            booking.AddServiceUse(1, "Spa", 150000m, 1);

            var invoice = new Invoice(booking, 0.0m);
            invoice.MarkAsPaid("Cash");

            // Act
            var points = await _loyaltyService.CalculatePointsEarnedAsync(invoice);

            // Assert
            // Tính toán:
            // - Tiền phòng: 1,000,000 / 20,000 = 50 điểm
            // - Dịch vụ: 150,000 / 10,000 = 15 điểm
            // - Hệ số nhân Silver: 1.0
            // Tổng: (50 + 15) * 1.0 = 65 điểm
            Assert.Equal(65, points);
        }

        [Fact]
        public async Task CalculatePointsEarned_PaidInvoice_PlatinumMember_ShouldApplyMultiplier()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var customer = new Customer("VIP Guest", "g@t.com", "123", "456");
            customer.AddPoints(16000); // Đã là Platinum -> multiplier 1.5x
            
            var booking = new Booking(1, 101, range, 500000m) { Customer = customer }; // Room charge = 1M
            booking.CheckIn();
            booking.AddServiceUse(1, "Spa", 100000m, 1); // 100K service

            var invoice = new Invoice(booking, 0.0m);
            invoice.MarkAsPaid("Cash");

            // Act
            var points = await _loyaltyService.CalculatePointsEarnedAsync(invoice);

            // Assert
            // Tính toán:
            // - Tiền phòng: 1,000,000 / 20,000 = 50 điểm
            // - Dịch vụ: 100,000 / 10,000 = 10 điểm
            // - Base points = 60
            // - Platinum multiplier: 1.5x
            // Tổng: 60 * 1.5 = 90 điểm
            Assert.Equal(90, points);
        }

        [Fact]
        public async Task ProcessLoyaltyPoints_ShouldAddPointsAndUpgradeCustomerTier()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(10)); // 10 ngày
            var customer = new Customer("Growing Member", "member@test.com", "098234", "123");
            customer.AddPoints(4800); // Hạng Silver (Gần lên Gold 5000)

            var booking = new Booking(1, 101, range, 1000000m) { Customer = customer }; // Room charge = 10M
            booking.CheckIn();

            var invoice = new Invoice(booking, 0.0m);
            invoice.MarkAsPaid("Credit Card");

            // Act
            // Tiền phòng 10M => 10,000,000 / 20,000 = 500 điểm thưởng
            var processed = await _loyaltyService.ProcessLoyaltyPointsForInvoiceAsync(customer, invoice);

            // Assert
            Assert.True(processed);
            Assert.Equal(5300, customer.LoyaltyPoints); // 4800 + 500 = 5300
            Assert.Equal(LoyaltyTier.Gold, customer.Tier); // Tự động thăng hạng từ Silver lên Gold vì > 5000đ
        }
    }
}
