using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Application.DTOs;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện định nghĩa các nghiệp vụ Quản lý Khách Hàng (CRUD).
    /// </summary>
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<PagedResultDto<CustomerDto>> GetPagedCustomersAsync(string searchTerm, int pageNumber, int pageSize);
        Task<CustomerDto> GetCustomerByIdAsync(int id);
        Task<bool> CreateCustomerAsync(CustomerCreateDto customer);
        Task<bool> UpdateCustomerAsync(CustomerUpdateDto customer);
        Task<bool> DeleteCustomerAsync(int id);
    }
}
