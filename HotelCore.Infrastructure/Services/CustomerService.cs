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
    /// Triển khai dịch vụ Quản lý Khách hàng sử dụng Dapper (CRUD).
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly string _connectionString;

        public CustomerService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is not configured.");
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Customers ORDER BY CustomerId DESC";
            return await db.QueryAsync<CustomerDto>(sql);
        }

        public async Task<PagedResultDto<CustomerDto>> GetPagedCustomersAsync(string searchTerm, int pageNumber, int pageSize)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string countSql = "SELECT COUNT(*) FROM Customers";
            string dataSql = @"SELECT * FROM Customers 
                               ORDER BY CustomerId DESC 
                               OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                countSql += " WHERE FullName LIKE @Search OR Phone LIKE @Search OR IdCardNumber LIKE @Search";
                dataSql = @"SELECT * FROM Customers 
                            WHERE FullName LIKE @Search OR Phone LIKE @Search OR IdCardNumber LIKE @Search 
                            ORDER BY CustomerId DESC 
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            }

            var offset = (pageNumber - 1) * pageSize;
            var totalCount = await db.ExecuteScalarAsync<int>(countSql, new { Search = $"%{searchTerm}%" });
            var items = await db.QueryAsync<CustomerDto>(dataSql, new { Search = $"%{searchTerm}%", Offset = offset, PageSize = pageSize });

            return new PagedResultDto<CustomerDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT * FROM Customers WHERE CustomerId = @Id";
            return await db.QuerySingleOrDefaultAsync<CustomerDto>(sql, new { Id = id });
        }

        public async Task<bool> CreateCustomerAsync(CustomerCreateDto customer)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                INSERT INTO Customers (FullName, Gender, DOB, IdCardNumber, Address, Phone, Email)
                VALUES (@FullName, @Gender, @DOB, @IdCardNumber, @Address, @Phone, @Email)";
            
            int rowsAffected = await db.ExecuteAsync(sql, customer);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateCustomerAsync(CustomerUpdateDto customer)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                UPDATE Customers 
                SET FullName = @FullName, 
                    Gender = @Gender, 
                    DOB = @DOB, 
                    IdCardNumber = @IdCardNumber, 
                    Address = @Address, 
                    Phone = @Phone, 
                    Email = @Email
                WHERE CustomerId = @CustomerId";
            
            int rowsAffected = await db.ExecuteAsync(sql, customer);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            try
            {
                string sql = "DELETE FROM Customers WHERE CustomerId = @Id";
                int rowsAffected = await db.ExecuteAsync(sql, new { Id = id });
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                // Bắt lỗi ràng buộc khóa ngoại (nếu khách hàng đã có hóa đơn/đặt phòng)
                Console.WriteLine($"Không thể xóa khách hàng: {ex.Message}");
                return false;
            }
        }
    }
}
