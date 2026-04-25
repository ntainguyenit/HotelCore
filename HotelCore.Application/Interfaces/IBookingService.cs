using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Application.DTOs;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện định nghĩa các nghiệp vụ liên quan đến Đặt phòng (Booking).
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Lấy danh sách khách hàng để hiển thị lên Form.
        /// </summary>
        Task<IEnumerable<CustomerDropdownDto>> GetCustomersAsync();

        /// <summary>
        /// Lấy danh sách các phòng ĐANG TRỐNG (Available) để hiển thị lên Form.
        /// </summary>
        Task<IEnumerable<RoomDropdownDto>> GetAvailableRoomsAsync();

        /// <summary>
        /// Xử lý tạo Đặt phòng mới (Sử dụng SqlTransaction để đảm bảo toàn vẹn dữ liệu).
        /// </summary>
        /// <param name="request">Thông tin đặt phòng từ người dùng.</param>
        /// <returns>True nếu thành công, False nếu thất bại.</returns>
        Task<bool> CreateBookingAsync(BookingRequestDto request);
    }
}
