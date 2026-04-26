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
    /// Triển khai các nghiệp vụ quản lý dịch vụ sử dụng Dapper
    /// </summary>
    public class ServiceService : IServiceService
    {
        private readonly string _connectionString;

        public ServiceService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        /// <summary>
        /// Lấy tất cả danh sách dịch vụ, có hỗ trợ tìm kiếm
        /// </summary>
        public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync(string searchTerm = null)
        {
            using var db = CreateConnection();
            string sql = "SELECT * FROM Services";
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += " WHERE ServiceName LIKE @Search";
                return await db.QueryAsync<ServiceDto>(sql, new { Search = $"%{searchTerm}%" });
            }

            return await db.QueryAsync<ServiceDto>(sql);
        }

        public async Task<PagedResultDto<ServiceDto>> GetPagedServicesAsync(string searchTerm, int pageNumber, int pageSize)
        {
            using var db = CreateConnection();
            string countSql = "SELECT COUNT(*) FROM Services";
            string dataSql = @"SELECT * FROM Services 
                               ORDER BY ServiceId 
                               OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                countSql += " WHERE ServiceName LIKE @Search";
                dataSql = @"SELECT * FROM Services 
                            WHERE ServiceName LIKE @Search 
                            ORDER BY ServiceId 
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            }

            var offset = (pageNumber - 1) * pageSize;
            var totalCount = await db.ExecuteScalarAsync<int>(countSql, new { Search = $"%{searchTerm}%" });
            var items = await db.QueryAsync<ServiceDto>(dataSql, new { Search = $"%{searchTerm}%", Offset = offset, PageSize = pageSize });

            return new PagedResultDto<ServiceDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Lấy thông tin chi tiết một dịch vụ
        /// </summary>
        public async Task<ServiceDto> GetServiceByIdAsync(int id)
        {
            using var db = CreateConnection();
            string sql = "SELECT * FROM Services WHERE ServiceId = @Id";
            return await db.QueryFirstOrDefaultAsync<ServiceDto>(sql, new { Id = id });
        }

        /// <summary>
        /// Thêm mới một dịch vụ vào hệ thống
        /// </summary>
        public async Task<bool> CreateServiceAsync(ServiceCreateDto serviceDto)
        {
            using var db = CreateConnection();
            string sql = @"INSERT INTO Services (ServiceName, Unit, Price, Status) 
                           VALUES (@ServiceName, @Unit, @Price, @Status)";
            
            int rows = await db.ExecuteAsync(sql, serviceDto);
            return rows > 0;
        }

        /// <summary>
        /// Cập nhật thông tin dịch vụ hiện có
        /// </summary>
        public async Task<bool> UpdateServiceAsync(ServiceUpdateDto serviceDto)
        {
            using var db = CreateConnection();
            string sql = @"UPDATE Services 
                           SET ServiceName = @ServiceName, 
                               Unit = @Unit, 
                               Price = @Price, 
                               Status = @Status 
                           WHERE ServiceId = @ServiceId";
            
            int rows = await db.ExecuteAsync(sql, serviceDto);
            return rows > 0;
        }

        /// <summary>
        /// Xóa dịch vụ theo ID
        /// </summary>
        public async Task<bool> DeleteServiceAsync(int id)
        {
            using var db = CreateConnection();
            string sql = "DELETE FROM Services WHERE ServiceId = @Id";
            
            try 
            {
                int rows = await db.ExecuteAsync(sql, new { Id = id });
                return rows > 0;
            }
            catch (SqlException ex)
            {
                // Nếu có lỗi ràng buộc khóa ngoại (dịch vụ đã có trong hóa đơn), 
                // có thể chuyển sang cập nhật Status = 0
                if (ex.Number == 547) 
                {
                    string updateSql = "UPDATE Services SET Status = 0 WHERE ServiceId = @Id";
                    await db.ExecuteAsync(updateSql, new { Id = id });
                    return true;
                }
                throw;
            }
        }
    }
}
