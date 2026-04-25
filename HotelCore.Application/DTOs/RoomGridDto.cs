namespace HotelCore.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) chứa thông tin chi tiết của một phòng 
    /// để hiển thị trên Sơ đồ lưới (Room Grid).
    /// </summary>
    public class RoomGridDto
    {
        /// <summary>
        /// Số hiệu phòng (vd: 101, 202).
        /// </summary>
        public string? RoomNumber { get; set; }

        /// <summary>
        /// Tầng của phòng (vd: 1, 2, 3).
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// Tên loại phòng (vd: Standard, VIP, Suite).
        /// </summary>
        public string? TypeName { get; set; }

        /// <summary>
        /// Sức chứa tối đa của phòng (số lượng người).
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Trạng thái hiện tại của phòng (Available, Occupied, Cleaning, Maintenance).
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Tên của khách hàng đang thuê phòng (nếu có). Null nếu phòng trống.
        /// </summary>
        public string? CurrentCustomerName { get; set; }
    }
}
