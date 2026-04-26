using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HotelCore.Application.DTOs
{
    public class SystemSettingsDto
    {
        // General
        [Required(ErrorMessage = "Tên khách sạn không được để trống")]
        [Display(Name = "Tên khách sạn")]
        public string HotelName { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        public string HotelAddress { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Display(Name = "Số điện thoại")]
        public string HotelPhone { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email liên hệ")]
        public string HotelEmail { get; set; }

        // Financial
        [Required(ErrorMessage = "Thuế suất VAT không được để trống")]
        [Range(0, 100, ErrorMessage = "Thuế suất phải từ 0-100%")]
        [Display(Name = "Thuế suất VAT (%)")]
        public decimal VatRate { get; set; }

        [Display(Name = "Đơn vị tiền tệ")]
        public string Currency { get; set; }

        [Range(0, 100, ErrorMessage = "Phí phục vụ phải từ 0-100%")]
        [Display(Name = "Phí phục vụ (%)")]
        public decimal ServiceFee { get; set; }

        // Branding
        [Display(Name = "Đường dẫn Logo")]
        public string LogoPath { get; set; }

        [Display(Name = "Thay đổi Logo")]
        public IFormFile LogoFile { get; set; }
    }
}
