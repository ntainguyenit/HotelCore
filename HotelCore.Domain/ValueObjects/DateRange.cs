using System;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Domain.ValueObjects
{
    /// <summary>
    /// Đối tượng giá trị (Value Object) đại diện cho khoảng thời gian nhận phòng và trả phòng.
    /// Đảm bảo tính bất biến (Immutability) và tự động xác thực tính hợp lệ của ngày đặt phòng.
    /// </summary>
    public class DateRange : IEquatable<DateRange>
    {
        public DateTime CheckInDate { get; }
        public DateTime CheckOutDate { get; }

        public DateRange(DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkInDate.Date < DateTime.Today)
            {
                throw new HotelDomainException("Ngày nhận phòng (Check-in) không thể nằm trong quá khứ.");
            }

            if (checkOutDate.Date <= checkInDate.Date)
            {
                throw new HotelDomainException("Ngày trả phòng (Check-out) phải diễn ra sau ngày nhận phòng ít nhất 1 ngày.");
            }

            CheckInDate = checkInDate.Date;
            CheckOutDate = checkOutDate.Date;
        }

        /// <summary>
        /// Tổng số ngày lưu trú thực tế.
        /// </summary>
        public int DurationInDays => (CheckOutDate - CheckInDate).Days;

        /// <summary>
        /// Kiểm tra xem khoảng thời gian này có bị chồng lấn (overlap) với một khoảng thời gian khác hay không.
        /// Sử dụng công thức toán học logic: (StartA < EndB) AND (EndA > StartB)
        /// </summary>
        public bool OverlapsWith(DateRange other)
        {
            if (other == null) return false;
            return CheckInDate < other.CheckOutDate && CheckOutDate > other.CheckInDate;
        }

        /// <summary>
        /// Kiểm tra xem một ngày cụ thể có nằm trong khoảng thời gian lưu trú hay không.
        /// </summary>
        public bool Contains(DateTime date)
        {
            var target = date.Date;
            return target >= CheckInDate && target < CheckOutDate;
        }

        public bool Equals(DateRange other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return CheckInDate == other.CheckInDate && CheckOutDate == other.CheckOutDate;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DateRange);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CheckInDate, CheckOutDate);
        }

        public static bool operator ==(DateRange left, DateRange right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(DateRange left, DateRange right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{CheckInDate:dd/MM/yyyy} - {CheckOutDate:dd/MM/yyyy} ({DurationInDays} ngày)";
        }
    }
}
