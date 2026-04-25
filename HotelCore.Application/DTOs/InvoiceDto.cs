using System;
using System.Collections.Generic;

namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// DTO hiển thị danh sách hóa đơn lịch sử.
    /// </summary>
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public string? CustomerName { get; set; }
        public string? RoomNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PaymentMethod { get; set; }
    }

    /// <summary>
    /// DTO chứa thông tin chi tiết để hiển thị trang Thanh Toán (Checkout).
    /// </summary>
    public class CheckoutViewDto
    {
        public int BookingId { get; set; }
        public string? CustomerName { get; set; }
        public string? RoomNumber { get; set; }
        public string? RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalDays { get; set; }
        public decimal RoomPricePerDay { get; set; }
        public decimal RoomTotal { get; set; }
        public decimal ServiceTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<InvoiceServiceDetailDto> Services { get; set; } = new List<InvoiceServiceDetailDto>();
    }

    public class InvoiceServiceDetailDto
    {
        public string? ServiceName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal => Quantity * Price;
    }

    /// <summary>
    /// DTO dùng để gửi yêu cầu thanh toán từ Form.
    /// </summary>
    public class InvoiceCreateDto
    {
        public int BookingId { get; set; }
        public decimal RoomTotal { get; set; }
        public decimal ServiceTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PaymentMethod { get; set; } // Cash, Credit Card, Bank Transfer
    }
}
