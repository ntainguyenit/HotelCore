using System;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Domain.Entities
{
    /// <summary>
    /// Thực thể đại diện cho Phòng trong khách sạn.
    /// Quản lý thông tin số phòng, loại phòng, tầng và trạng thái vận hành.
    /// </summary>
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public int RoomTypeId { get; set; }
        public string TypeName { get; set; }
        public decimal BasePrice { get; set; }
        public int Floor { get; set; }
        public RoomStatus Status { get; private set; }
        public string Notes { get; set; }

        public Room()
        {
            Status = RoomStatus.Available;
        }

        public Room(string roomNumber, int roomTypeId, decimal basePrice, int floor) : this()
        {
            if (string.IsNullOrWhiteSpace(roomNumber))
                throw new HotelDomainException("Số phòng không được trống.");
            if (basePrice < 0)
                throw new HotelDomainException("Giá cơ bản của phòng không thể là số âm.");
            if (floor < 1)
                throw new HotelDomainException("Tầng của phòng phải từ 1 trở lên.");

            RoomNumber = roomNumber;
            RoomTypeId = roomTypeId;
            BasePrice = basePrice;
            Floor = floor;
        }

        /// <summary>
        /// Chuyển trạng thái phòng khi có khách Check-in.
        /// </summary>
        public void Occupy()
        {
            if (Status != RoomStatus.Available)
                throw new HotelDomainException($"Không thể nhận phòng. Trạng thái phòng hiện tại là: {Status}");

            Status = RoomStatus.Occupied;
        }

        /// <summary>
        /// Khách Check-out, phòng cần được dọn dẹp trước khi sẵn sàng đón khách tiếp theo.
        /// </summary>
        public void ReleaseForCleaning()
        {
            if (Status != RoomStatus.Occupied)
                throw new HotelDomainException("Chỉ có phòng đang có khách mới có thể chuyển trạng thái dọn dẹp.");

            Status = RoomStatus.Cleaning;
        }

        /// <summary>
        /// Hoàn tất việc vệ sinh, phòng sẵn sàng đón khách.
        /// </summary>
        public void CompleteCleaning()
        {
            if (Status != RoomStatus.Cleaning)
                throw new HotelDomainException("Chỉ có phòng đang dọn dẹp mới có thể hoàn tất vệ sinh.");

            Status = RoomStatus.Available;
        }

        /// <summary>
        /// Chuyển phòng sang trạng thái bảo trì kỹ thuật.
        /// </summary>
        public void SetToMaintenance(string reason)
        {
            if (Status == RoomStatus.Occupied)
                throw new HotelDomainException("Không thể đưa phòng đang có khách thuê vào bảo trì.");

            Status = RoomStatus.Maintenance;
            Notes = $"Bảo trì: {reason}";
        }

        /// <summary>
        /// Hoàn tất bảo trì, đưa phòng hoạt động trở lại bình thường.
        /// </summary>
        public void CompleteMaintenance()
        {
            if (Status != RoomStatus.Maintenance)
                throw new HotelDomainException("Phòng không ở trong trạng thái bảo trì.");

            Status = RoomStatus.Available;
            Notes = string.Empty;
        }
    }
}
