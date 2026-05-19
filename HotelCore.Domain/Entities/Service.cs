using System;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Domain.Entities
{
    /// <summary>
    /// Thực thể đại diện cho Dịch vụ phụ trợ của khách sạn.
    /// Ví dụ: Spa, Ăn uống tại phòng, Giặt là, Thuê xe tự lái...
    /// </summary>
    public class Service
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Service() { }

        public Service(string serviceName, decimal price, string description = "")
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new HotelDomainException("Tên dịch vụ không được trống.");
            if (price < 0)
                throw new HotelDomainException("Giá dịch vụ không được phép là số âm.");

            ServiceName = serviceName;
            Price = price;
            Description = description;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new HotelDomainException("Đơn giá dịch vụ mới không thể âm.");
            Price = newPrice;
        }
    }
}
