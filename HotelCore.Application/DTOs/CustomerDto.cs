using System;

namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// DTO chứa thông tin Khách hàng để hiển thị danh sách (Read).
    /// </summary>
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string IdCardNumber { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
