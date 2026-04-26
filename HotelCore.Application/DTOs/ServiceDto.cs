using System.ComponentModel.DataAnnotations;

namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// DTO hiển thị thông tin dịch vụ
    /// </summary>
    public class ServiceDto
    {
        public int ServiceId { get; set; }

        [Display(Name = "Tên dịch vụ")]
        public string ServiceName { get; set; }

        [Display(Name = "Đơn vị tính")]
        public string Unit { get; set; }

        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }

        [Display(Name = "Trạng thái")]
        public bool Status { get; set; }

        public string StatusText => Status ? "Đang kinh doanh" : "Ngừng kinh doanh";
    }

    /// <summary>
    /// DTO dùng để tạo mới dịch vụ
    /// </summary>
    public class ServiceCreateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ")]
        [Display(Name = "Tên dịch vụ")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đơn vị tính")]
        [Display(Name = "Đơn vị tính")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đơn giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool Status { get; set; } = true;
    }

    /// <summary>
    /// DTO dùng để cập nhật dịch vụ
    /// </summary>
    public class ServiceUpdateDto : ServiceCreateDto
    {
        public int ServiceId { get; set; }
    }
}
