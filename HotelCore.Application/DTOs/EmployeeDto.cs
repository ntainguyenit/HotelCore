using System;
using System.ComponentModel.DataAnnotations;

namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// DTO hiển thị danh sách nhân viên.
    /// </summary>
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? RoleName { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO chứa thông tin chức vụ để đổ vào dropdown.
    /// </summary>
    public class RoleDropdownDto
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }

    /// <summary>
    /// DTO tạo mới nhân viên.
    /// </summary>
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [Display(Name = "Họ và Tên")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chức vụ.")]
        [Display(Name = "Chức vụ")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Lương không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Lương phải là số dương.")]
        [Display(Name = "Lương cơ bản")]
        public decimal Salary { get; set; }

        [Display(Name = "Đang làm việc")]
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO cập nhật nhân viên.
    /// </summary>
    public class EmployeeUpdateDto : EmployeeCreateDto
    {
        public int EmployeeId { get; set; }
    }
}
