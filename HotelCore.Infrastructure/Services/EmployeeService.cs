using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelCore.Infrastructure.Services
{
    /// <summary>
    /// Triển khai dịch vụ quản lý nhân viên bằng Dapper.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is not configured.");
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                SELECT e.EmployeeId, e.FullName, e.Phone, e.Email, e.BaseSalary AS Salary, e.Status AS IsActive, r.RoleName
                FROM Employees e
                LEFT JOIN Roles r ON e.RoleId = r.RoleId
                ORDER BY e.EmployeeId DESC";
            return await db.QueryAsync<EmployeeDto>(sql);
        }

        public async Task<EmployeeUpdateDto?> GetEmployeeByIdAsync(int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT EmployeeId, HotelId, RoleId, FullName, Phone, Email, BaseSalary AS Salary, Status AS IsActive FROM Employees WHERE EmployeeId = @Id";
            return await db.QuerySingleOrDefaultAsync<EmployeeUpdateDto>(sql, new { Id = id });
        }

        public async Task<IEnumerable<RoleDropdownDto>> GetRolesAsync()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT RoleId, RoleName FROM Roles";
            return await db.QueryAsync<RoleDropdownDto>(sql);
        }

        public async Task<bool> CreateEmployeeAsync(EmployeeCreateDto employee)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                INSERT INTO Employees (HotelId, RoleId, FullName, Phone, Email, BaseSalary, Status)
                VALUES (1, @RoleId, @FullName, @Phone, @Email, @Salary, @IsActive)";
            // Lưu ý: HotelId tạm thời để 1
            int rows = await db.ExecuteAsync(sql, employee);
            return rows > 0;
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeUpdateDto employee)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                UPDATE Employees 
                SET RoleId = @RoleId, FullName = @FullName, Phone = @Phone, Email = @Email, 
                    BaseSalary = @Salary, Status = @IsActive
                WHERE EmployeeId = @EmployeeId";
            int rows = await db.ExecuteAsync(sql, employee);
            return rows > 0;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            try
            {
                string sql = "DELETE FROM Employees WHERE EmployeeId = @Id";
                int rows = await db.ExecuteAsync(sql, new { Id = id });
                return rows > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
