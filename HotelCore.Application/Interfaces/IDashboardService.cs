using System.Threading.Tasks;
using HotelCore.Application.DTOs;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện định nghĩa các nghiệp vụ (Service) liên quan đến trang Dashboard.
    /// Tuân thủ nguyên lý Dependency Inversion trong Clean Architecture.
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Truy vấn và tính toán toàn bộ dữ liệu thống kê tổng quan 
        /// và danh sách lưới phòng từ cơ sở dữ liệu.
        /// </summary>
        /// <returns>Đối tượng DashboardOverviewDto chứa toàn bộ số liệu.</returns>
        Task<DashboardOverviewDto> GetDashboardOverviewAsync();
    }
}
