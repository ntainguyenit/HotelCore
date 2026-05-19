using System;
using System.Collections.Generic;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Domain.Entities
{
    /// <summary>
    /// Thực thể đại diện cho Hóa đơn thanh toán.
    /// Thực hiện tổng hợp chi phí phòng, dịch vụ, thuế VAT và chiết khấu phân hạng thành viên.
    /// </summary>
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
        public string InvoiceNumber { get; private set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public decimal RoomCharges { get; private set; }
        public decimal ServiceCharges { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TaxRate { get; private set; } = 0.10m; // 10% VAT mặc định
        public decimal TotalAmount { get; private set; }
        public bool IsPaid { get; private set; } = false;
        public string PaymentMethod { get; set; }

        public Invoice()
        {
            InvoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
        }

        public Invoice(Booking booking, decimal discountRate = 0) : this()
        {
            Booking = booking ?? throw new HotelDomainException("Thông tin Booking không thể trống để xuất Hóa đơn.");
            BookingId = booking.BookingId;

            CalculateCharges(discountRate);
        }

        /// <summary>
        /// Thực hiện toàn bộ logic nghiệp vụ tính toán chi phí hóa đơn.
        /// Áp dụng thuế suất và tỷ lệ chiết khấu cụ thể.
        /// </summary>
        public void CalculateCharges(decimal discountRate)
        {
            if (discountRate < 0 || discountRate > 1.0m)
                throw new HotelDomainException("Tỷ lệ giảm giá phải từ 0% đến 100% (0.0 đến 1.0).");

            RoomCharges = Booking.CalculateBaseRoomCharge();
            
            decimal serviceTotal = 0;
            foreach (var item in Booking.ServicesUsed)
            {
                serviceTotal += item.TotalCost;
            }
            ServiceCharges = serviceTotal;

            var baseTotal = RoomCharges + ServiceCharges;
            DiscountAmount = Math.Round(baseTotal * discountRate, 2);

            var taxableAmount = baseTotal - DiscountAmount;
            var taxAmount = Math.Round(taxableAmount * TaxRate, 2);

            TotalAmount = taxableAmount + taxAmount;
        }

        /// <summary>
        /// Ghi nhận thanh toán hóa đơn.
        /// </summary>
        public void MarkAsPaid(string paymentMethod)
        {
            if (IsPaid)
                throw new HotelDomainException("Hóa đơn này đã được thanh toán trước đó.");
            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new HotelDomainException("Phương thức thanh toán không được để trống.");

            IsPaid = true;
            PaymentMethod = paymentMethod;
            Booking.CheckOut();
        }
    }
}
