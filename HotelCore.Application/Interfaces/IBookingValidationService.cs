using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Domain.Entities;
using HotelCore.Domain.ValueObjects;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện xử lý kiểm tra các quy tắc nghiệp vụ đặt phòng nâng cao nhằm tránh xung đột phòng.
    /// </summary>
    public interface IBookingValidationService
    {
        /// <summary>
        /// Kiểm tra xem phòng cụ thể có bị trùng lịch trong khoảng thời gian dự kiến đặt hay không.
        /// </summary>
        Task<bool> IsRoomAvailableForPeriodAsync(int roomId, DateRange desiredPeriod, IEnumerable<Booking> activeBookings);

        /// <summary>
        /// Xác thực một yêu cầu đặt phòng đầy đủ. Ném ra ngoại lệ nếu vi phạm quy định.
        /// </summary>
        Task ValidateBookingRequestAsync(Customer customer, Room room, DateRange period, IEnumerable<Booking> activeBookings);
    }
}
