using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Application.Services;
using HotelCore.Domain.Entities;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;
using HotelCore.Domain.ValueObjects;
using Xunit;

namespace HotelCore.Tests.Application
{
    /// <summary>
    /// Các bài kiểm thử cho dịch vụ BookingValidationService.
    /// </summary>
    public class BookingValidationTests
    {
        private readonly BookingValidationService _validationService;
        private readonly List<Booking> _activeBookings;

        public BookingValidationTests()
        {
            _validationService = new BookingValidationService();
            
            // Setup mock data
            var today = DateTime.Today;
            _activeBookings = new List<Booking>
            {
                // Phòng 101 có lịch từ hôm nay đến 3 ngày sau
                new Booking(1, 101, new DateRange(today, today.AddDays(3)), 500000m),
                // Phòng 102 có lịch từ 4 ngày sau đến 6 ngày sau
                new Booking(2, 102, new DateRange(today.AddDays(4), today.AddDays(6)), 600000m)
            };
        }

        [Fact]
        public async Task IsRoomAvailable_RoomNoBookings_ShouldReturnTrue()
        {
            // Arrange
            var period = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));

            // Act
            var available = await _validationService.IsRoomAvailableForPeriodAsync(105, period, _activeBookings);

            // Assert
            Assert.True(available);
        }

        [Fact]
        public async Task IsRoomAvailable_RoomHasOverlapBooking_ShouldReturnFalse()
        {
            // Arrange
            // Trùng 1 ngày với booking phòng 101 (hôm nay -> 3 ngày sau)
            var period = new DateRange(DateTime.Today.AddDays(2), DateTime.Today.AddDays(4));

            // Act
            var available = await _validationService.IsRoomAvailableForPeriodAsync(101, period, _activeBookings);

            // Assert
            Assert.False(available);
        }

        [Fact]
        public async Task ValidateBookingRequest_RoomInMaintenance_ShouldThrowHotelDomainException()
        {
            // Arrange
            var customer = new Customer("Guest", "g@t.com", "123", "456");
            var room = new Room("202", 1, 500000m, 2);
            room.SetToMaintenance("Hỏng điều hòa");

            var period = new DateRange(DateTime.Today, DateTime.Today.AddDays(2));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HotelDomainException>(() => 
                _validationService.ValidateBookingRequestAsync(customer, room, period, _activeBookings));
            
            Assert.Contains("đang trong thời gian bảo trì kỹ thuật", ex.Message);
        }

        [Fact]
        public async Task ValidateBookingRequest_StayTooLong_ShouldThrowHotelDomainException()
        {
            // Arrange
            var customer = new Customer("Guest", "g@t.com", "123", "456");
            var room = new Room("202", 1, 500000m, 2);
            // 35 ngày lưu trú (vượt giới hạn 30 ngày)
            var period = new DateRange(DateTime.Today, DateTime.Today.AddDays(35));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HotelDomainException>(() => 
                _validationService.ValidateBookingRequestAsync(customer, room, period, _activeBookings));
            
            Assert.Contains("không được phép vượt quá 30 ngày lưu trú", ex.Message);
        }

        [Fact]
        public async Task ValidateBookingRequest_Overlap_ShouldThrowHotelDomainException()
        {
            // Arrange
            var customer = new Customer("Guest", "g@t.com", "123", "456");
            var room = new Room("101", 1, 500000m, 1) { RoomId = 101 };
            // Trùng lịch với booking 101 đang có sẵn
            var period = new DateRange(DateTime.Today.AddDays(1), DateTime.Today.AddDays(2));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HotelDomainException>(() => 
                _validationService.ValidateBookingRequestAsync(customer, room, period, _activeBookings));
            
            Assert.Contains("đã có khách đặt lịch trùng", ex.Message);
        }

        [Fact]
        public async Task ValidateBookingRequest_ValidRequest_ShouldPassWithoutExceptions()
        {
            // Arrange
            var customer = new Customer("Guest", "g@t.com", "123", "456");
            var room = new Room("102", 1, 600000m, 1) { RoomId = 102 };
            // Phòng 102 có sẵn lịch từ ngày 4->6. Chúng ta đặt từ ngày 0->3 (không trùng!)
            var period = new DateRange(DateTime.Today, DateTime.Today.AddDays(3));

            // Act & Assert (Không có exception nào ném ra)
            await _validationService.ValidateBookingRequestAsync(customer, room, period, _activeBookings);
        }
    }
}
