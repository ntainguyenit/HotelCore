using System;
using System.ComponentModel.DataAnnotations;

namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// DTO nhận dữ liệu đầu vào từ Form Đặt Phòng của giao diện người dùng.
    /// Có sử dụng DataAnnotations để Validate phía Server.
    /// </summary>
    public class BookingRequestDto
    {
        /// <summary>
        /// Mã khách hàng đã chọn.
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn khách hàng.")]
        [Display(Name = "Khách Hàng")]
        public int CustomerId { get; set; }

        /// <summary>
        /// Mã phòng đã chọn (chỉ hiển thị phòng trống).
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn phòng.")]
        [Display(Name = "Phòng")]
        public int RoomId { get; set; }

        /// <summary>
        /// Ngày dự kiến nhận phòng.
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày Nhận Phòng (Check-in)")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Ngày dự kiến trả phòng.
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày Trả Phòng (Check-out)")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        /// <summary>
        /// Ghi chú thêm từ khách hàng (không bắt buộc).
        /// </summary>
        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
        public string? Notes { get; set; }
    }
}
