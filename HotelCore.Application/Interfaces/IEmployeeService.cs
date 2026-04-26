using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCore.Application.DTOs;

namespace HotelCore.Application.Interfaces
{
    /// <summary>
    /// Giao diện định nghĩa các nghiệp vụ Quản lý Nhân viên.
    /// </summary>
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<PagedResultDto<EmployeeDto>> GetPagedEmployeesAsync(string searchTerm, int pageNumber, int pageSize);
        Task<EmployeeUpdateDto> GetEmployeeByIdAsync(int id);
        Task<IEnumerable<RoleDropdownDto>> GetRolesAsync();
        Task<bool> CreateEmployeeAsync(EmployeeCreateDto employee);
        Task<bool> UpdateEmployeeAsync(EmployeeUpdateDto employee);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
