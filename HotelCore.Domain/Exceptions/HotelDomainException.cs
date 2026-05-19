using System;

namespace HotelCore.Domain.Exceptions
{
    /// <summary>
    /// Ngoại lệ tùy chỉnh đại diện cho các vi phạm quy tắc nghiệp vụ (invariants) trong tầng Domain.
    /// Giúp phân biệt lỗi hệ thống hệ quản trị cơ sở dữ liệu với lỗi vi phạm logic kinh doanh.
    /// </summary>
    public class HotelDomainException : Exception
    {
        public HotelDomainException() : base() { }

        public HotelDomainException(string message) : base(message) { }

        public HotelDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
