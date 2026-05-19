using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelCore.Application.Interfaces;
using HotelCore.Domain.Entities;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;
using HotelCore.Domain.ValueObjects;

namespace HotelCore.Application.Services
{
    /// <summary>
    /// Triển khai dịch vụ xác thực đặt phòng chuyên sâu.
    /// Kiểm tra tính sẵn sàng của phòng, xung đột lịch đặt và các giới hạn nghiệp vụ khách sạn.
    /// </summary>
    public class BookingValidationService : IBookingValidationService
    {
        public async Task<bool> IsRoomAvailableForPeriodAsync(
            int roomId, 
            DateRange desiredPeriod, 
            IEnumerable<Booking> activeBookings)
        {
            if (desiredPeriod == null) return false;

            // Lọc ra các booking của chính phòng này, đang hoạt động (không bị hủy)
            var roomBookings = activeBookings.Where(b => 
                b.RoomId == roomId && 
                b.Status != BookingStatus.Cancelled && 
                b.Status != BookingStatus.Completed);

            // Kiểm tra xem có bất kỳ booking nào bị chồng chéo thời gian hay không
            foreach (var booking in roomBookings)
            {
                if (booking.DateRange != null && booking.DateRange.OverlapsWith(desiredPeriod))
                {
                    return await Task.FromResult(false); // Bị trùng lịch!
                }
            }

            return await Task.FromResult(true); // Sẵn sàng đặt phòng
        }

        public async Task ValidateBookingRequestAsync(
            Customer customer, 
            Room room, 
            DateRange period, 
            IEnumerable<Booking> activeBookings)
        {
            if (customer == null)
                throw new HotelDomainException("Thông tin khách hàng là bắt buộc và không được trống.");

            if (room == null)
                throw new HotelDomainException("Thông tin phòng đặt là bắt buộc và không được trống.");

            if (period == null)
                throw new HotelDomainException("Khoảng thời gian lưu trú là bắt buộc.");

            // 1. Kiểm tra trạng thái hoạt động của phòng
            if (room.Status == RoomStatus.Maintenance)
                throw new HotelDomainException($"Phòng {room.RoomNumber} đang trong thời gian bảo trì kỹ thuật, không thể cho thuê.");

            // 2. Kiểm tra giới hạn số lượng ngày đặt tối đa của một lượt đặt phòng (vd: tối đa 30 ngày)
            const int MaxStayDurationDays = 30;
            if (period.DurationInDays > MaxStayDurationDays)
                throw new HotelDomainException($"Mỗi lượt đặt phòng không được phép vượt quá {MaxStayDurationDays} ngày lưu trú.");

            // 3. Kiểm tra xem phòng có bị trùng lịch trong khoảng thời gian yêu cầu hay không
            bool isAvailable = await IsRoomAvailableForPeriodAsync(room.RoomId, period, activeBookings);
            if (!isAvailable)
            {
                throw new HotelDomainException($"Phòng {room.RoomNumber} đã có khách đặt lịch trùng với khoảng thời gian từ {period.CheckInDate:dd/MM/yyyy} đến {period.CheckOutDate:dd/MM/yyyy}.");
            }
        }
    }
}
