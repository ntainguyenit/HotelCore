using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Application.DTOs;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện cung cấp các nghiệp vụ quản lý dịch vụ khách sạn
    /// </summary>
    public interface IServiceService
    {
        /// <summary>
        /// Lấy tất cả danh sách dịch vụ
        /// </summary>
        Task<IEnumerable<ServiceDto>> GetAllServicesAsync(string searchTerm = null);

        /// <summary>
        /// Lấy danh sách dịch vụ có phân trang
        /// </summary>
        Task<PagedResultDto<ServiceDto>> GetPagedServicesAsync(string searchTerm, int pageNumber, int pageSize);

        /// <summary>
        /// Lấy thông tin chi tiết một dịch vụ theo ID
        /// </summary>
        Task<ServiceDto> GetServiceByIdAsync(int id);

        /// <summary>
        /// Thêm mới một dịch vụ
        /// </summary>
        Task<bool> CreateServiceAsync(ServiceCreateDto serviceDto);

        /// <summary>
        /// Cập nhật thông tin dịch vụ
        /// </summary>
        Task<bool> UpdateServiceAsync(ServiceUpdateDto serviceDto);

        /// <summary>
        /// Xóa dịch vụ (hoặc chuyển trạng thái ngừng kinh doanh)
        /// </summary>
        Task<bool> DeleteServiceAsync(int id);
    }
}
