using System;
using System.Collections.Generic;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;
using HotelCore.Domain.ValueObjects;

namespace HotelCore.Domain.Entities
{
    /// <summary>
    /// Thực thể đại diện cho Lượt đặt phòng.
    /// Quản lý vòng đời thuê phòng, các dịch vụ gia tăng được sử dụng, và chi phí gốc.
    /// </summary>
    public class Booking
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int EmployeeId { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public DateRange DateRange { get; private set; }
        public decimal RoomPriceAtBooking { get; private set; }
        public BookingStatus Status { get; private set; }
        public string Notes { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;

        private readonly List<BookingServiceUse> _servicesUsed = new List<BookingServiceUse>();
        public IReadOnlyCollection<BookingServiceUse> ServicesUsed => _servicesUsed.AsReadOnly();

        public Booking()
        {
            Status = BookingStatus.Confirmed;
        }

        public Booking(int customerId, int roomId, DateRange dateRange, decimal roomPrice) : this()
        {
            if (customerId <= 0)
                throw new HotelDomainException("CustomerId không hợp lệ.");
            if (roomId <= 0)
                throw new HotelDomainException("RoomId không hợp lệ.");
            if (roomPrice < 0)
                throw new HotelDomainException("Giá phòng tại thời điểm đặt không thể âm.");

            CustomerId = customerId;
            RoomId = roomId;
            DateRange = dateRange ?? throw new HotelDomainException("Khoảng thời gian lưu trú không được để trống.");
            RoomPriceAtBooking = roomPrice;
        }

        /// <summary>
        /// Tính tổng tiền phòng thuần túy chưa tính ưu đãi giảm giá (số ngày x đơn giá).
        /// </summary>
        public decimal CalculateBaseRoomCharge()
        {
            return DateRange.DurationInDays * RoomPriceAtBooking;
        }

        /// <summary>
        /// Thêm dịch vụ phụ trợ khách hàng sử dụng khi đang lưu trú.
        /// </summary>
        public void AddServiceUse(int serviceId, string serviceName, decimal price, int quantity)
        {
            if (Status != BookingStatus.CheckedIn && Status != BookingStatus.Confirmed)
                throw new HotelDomainException("Chỉ có thể thêm dịch vụ cho lượt đặt phòng đang hoạt động hoặc chuẩn bị.");
            if (quantity <= 0)
                throw new HotelDomainException("Số lượng dịch vụ phải lớn hơn 0.");
            if (price < 0)
                throw new HotelDomainException("Đơn giá dịch vụ không được phép là số âm.");

            var existing = _servicesUsed.Find(s => s.ServiceId == serviceId);
            if (existing != null)
            {
                existing.UpdateQuantity(existing.Quantity + quantity);
            }
            else
            {
                _servicesUsed.Add(new BookingServiceUse(serviceId, serviceName, price, quantity));
            }
        }

        /// <summary>
        /// Tiến hành nhận phòng (Check-in), cập nhật trạng thái đặt phòng.
        /// </summary>
        public void CheckIn()
        {
            if (Status != BookingStatus.Confirmed)
                throw new HotelDomainException($"Chỉ có thể Check-in cho lượt đặt phòng trạng thái Confirmed. Trạng thái hiện tại: {Status}");

            if (DateTime.Today < DateRange.CheckInDate)
                throw new HotelDomainException("Chưa đến ngày nhận phòng theo lịch đăng ký.");

            Status = BookingStatus.CheckedIn;
        }

        /// <summary>
        /// Tiến hành hoàn tất và trả phòng (Check-out).
        /// </summary>
        public void CheckOut()
        {
            if (Status != BookingStatus.CheckedIn)
                throw new HotelDomainException("Chỉ có thể Check-out khi khách đã nhận phòng.");

            Status = BookingStatus.Completed;
        }

        /// <summary>
        /// Hủy lượt đặt phòng.
        /// </summary>
        public void Cancel()
        {
            if (Status == BookingStatus.Completed)
                throw new HotelDomainException("Không thể hủy lượt đặt phòng đã hoàn tất lưu trú.");
            if (Status == BookingStatus.CheckedIn)
                throw new HotelDomainException("Khách đã nhận phòng thực tế, không thể hủy, vui lòng thực hiện thủ tục Check-out.");

            Status = BookingStatus.Cancelled;
        }
    }

    /// <summary>
    /// Thực thể phụ lưu vết việc sử dụng dịch vụ tại phòng.
    /// </summary>
    public class BookingServiceUse
    {
        public int ServiceId { get; }
        public string ServiceName { get; }
        public decimal Price { get; }
        public int Quantity { get; private set; }

        public BookingServiceUse(int serviceId, string serviceName, decimal price, int quantity)
        {
            ServiceId = serviceId;
            ServiceName = serviceName;
            Price = price;
            Quantity = quantity;
        }

        public decimal TotalCost => Price * Quantity;

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new HotelDomainException("Số lượng dịch vụ cập nhật phải lớn hơn 0.");
            Quantity = newQuantity;
        }
    }
}
