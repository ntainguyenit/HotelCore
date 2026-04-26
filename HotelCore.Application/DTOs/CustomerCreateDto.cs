using System;
using System.ComponentModel.DataAnnotations;

namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// DTO chứa thông tin để Tạo mới Khách hàng.
    /// Có Validate phía Server.
    /// </summary>
    public class CustomerCreateDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Số CMND/CCCD không được để trống.")]
        [StringLength(20)]
        [Display(Name = "CMND/CCCD")]
        public string IdCardNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// DTO chứa thông tin để Cập nhật Khách hàng (Kế thừa từ CreateDto và thêm ID).
    /// </summary>
    public class CustomerUpdateDto : CustomerCreateDto
    {
        [Required]
        public int CustomerId { get; set; }
    }
}
