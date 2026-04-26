using System;
using System.ComponentModel.DataAnnotations;

namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// DTO cho form Đăng nhập
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// DTO lưu trữ thông tin phiên làm việc của người dùng
    /// </summary>
    public class UserSessionDto
    {
        public int AccountId { get; set; }
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string HotelName { get; set; }
    }

    /// <summary>
    /// DTO hiển thị và chỉnh sửa Hồ sơ cá nhân
    /// </summary>
    public class UserProfileDto
    {
        public int EmployeeId { get; set; }
        
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Display(Name = "Chức vụ")]
        public string RoleName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? DOB { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
    }
}
