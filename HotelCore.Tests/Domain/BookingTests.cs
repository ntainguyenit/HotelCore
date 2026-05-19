using System;
using HotelCore.Domain.Entities;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;
using HotelCore.Domain.ValueObjects;
using Xunit;

namespace HotelCore.Tests.Domain
{
    /// <summary>
    /// Các bài kiểm thử đơn vị cho thực thể Booking và BookingServiceUse.
    /// </summary>
    public class BookingTests
    {
        [Fact]
        public void CreateBooking_ValidArguments_ShouldInitializeCorrectly()
        {
            // Arrange
            int customerId = 42;
            int roomId = 101;
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(3));
            decimal price = 500000m;

            // Act
            var booking = new Booking(customerId, roomId, range, price);

            // Assert
            Assert.Equal(customerId, booking.CustomerId);
            Assert.Equal(roomId, booking.RoomId);
            Assert.Equal(range, booking.DateRange);
            Assert.Equal(price, booking.RoomPriceAtBooking);
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
            Assert.Empty(booking.ServicesUsed);
        }

        [Fact]
        public void CalculateBaseRoomCharge_ShouldMultiplyPriceByDuration()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(4)); // 4 ngày lưu trú
            decimal price = 1200000m;
            var booking = new Booking(1, 1, range, price);

            // Act
            var total = booking.CalculateBaseRoomCharge();

            // Assert
            Assert.Equal(4800000m, total);
        }

        [Fact]
        public void AddServiceUse_NewService_ShouldAddToCollection()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var booking = new Booking(1, 1, range, 300000m);

            // Act
            booking.AddServiceUse(5, "Spa massage", 150000m, 2);

            // Assert
            Assert.Single(booking.ServicesUsed);
            var serviceUse = Assert.Single(booking.ServicesUsed);
            Assert.Equal(5, serviceUse.ServiceId);
            Assert.Equal("Spa massage", serviceUse.ServiceName);
            Assert.Equal(150000m, serviceUse.Price);
            Assert.Equal(2, serviceUse.Quantity);
            Assert.Equal(300000m, serviceUse.TotalCost);
        }

        [Fact]
        public void AddServiceUse_ExistingService_ShouldIncrementQuantity()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var booking = new Booking(1, 1, range, 300000m);
            booking.AddServiceUse(2, "Coca Cola", 15000m, 1);

            // Act
            booking.AddServiceUse(2, "Coca Cola", 15000m, 3); // Lấy thêm 3 lon nữa

            // Assert
            var serviceUse = Assert.Single(booking.ServicesUsed);
            Assert.Equal(4, serviceUse.Quantity);
            Assert.Equal(60000m, serviceUse.TotalCost);
        }

        [Fact]
        public void AddServiceUse_NegativePrice_ShouldThrowHotelDomainException()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var booking = new Booking(1, 1, range, 300000m);

            // Act & Assert
            Assert.Throws<HotelDomainException>(() => booking.AddServiceUse(1, "Free service?", -1000m, 1));
        }

        [Fact]
        public void CheckIn_ValidConfirmedBooking_ShouldChangeStatusToCheckedIn()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var booking = new Booking(1, 1, range, 300000m);

            // Act
            booking.CheckIn();

            // Assert
            Assert.Equal(BookingStatus.CheckedIn, booking.Status);
        }

        [Fact]
        public void CheckIn_FutureBookingDate_ShouldThrowHotelDomainException()
        {
            // Arrange
            var range = new DateRange(DateTime.Today.AddDays(2), DateTime.Today.AddDays(4)); // Bắt đầu từ kia
            var booking = new Booking(1, 1, range, 300000m);

            // Act & Assert
            var ex = Assert.Throws<HotelDomainException>(() => booking.CheckIn());
            Assert.Contains("Chưa đến ngày nhận phòng", ex.Message);
        }

        [Fact]
        public void CheckOut_CheckedInBooking_ShouldCompleteSuccessfully()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var booking = new Booking(1, 1, range, 300000m);
            booking.CheckIn();

            // Act
            booking.CheckOut();

            // Assert
            Assert.Equal(BookingStatus.Completed, booking.Status);
        }

        [Fact]
        public void CheckOut_ConfirmedButNotCheckedIn_ShouldThrowHotelDomainException()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var booking = new Booking(1, 1, range, 300000m);

            // Act & Assert
            var ex = Assert.Throws<HotelDomainException>(() => booking.CheckOut());
            Assert.Contains("Chỉ có thể Check-out khi khách đã nhận phòng", ex.Message);
        }

        [Fact]
        public void Cancel_ConfirmedBooking_ShouldChangeStatusToCancelled()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var booking = new Booking(1, 1, range, 300000m);

            // Act
            booking.Cancel();

            // Assert
            Assert.Equal(BookingStatus.Cancelled, booking.Status);
        }

        [Fact]
        public void Cancel_CheckedInBooking_ShouldThrowHotelDomainException()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));
            var booking = new Booking(1, 1, range, 300000m);
            booking.CheckIn();

            // Act & Assert
            var ex = Assert.Throws<HotelDomainException>(() => booking.Cancel());
            Assert.Contains("Khách đã nhận phòng thực tế, không thể hủy", ex.Message);
        }
    }
}
