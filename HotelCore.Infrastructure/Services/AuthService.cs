using System;
using System.Data;
using System.Threading.Tasks;
using BCrypt.Net;
using Dapper;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelCore.Infrastructure.Services
{
    /// <summary>
    /// Triển khai dịch vụ xác thực và quản lý tài khoản nhân viên
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly string _connectionString;

        public AuthService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        /// <summary>
        /// Xác thực đăng nhập
        /// </summary>
        public async Task<UserSessionDto> LoginAsync(string username, string password)
        {
            using var db = CreateConnection();
            string sql = @"SELECT a.AccountId, a.EmployeeId, a.Username, a.PasswordHash, 
                                  e.FullName, r.RoleName, h.HotelName
                           FROM Accounts a
                           JOIN Employees e ON a.EmployeeId = e.EmployeeId
                           JOIN Roles r ON e.RoleId = r.RoleId
                           JOIN Hotels h ON e.HotelId = h.HotelId
                           WHERE a.Username = @Username AND e.Status = 1";

            var user = await db.QueryFirstOrDefaultAsync<dynamic>(sql, new { Username = username });

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // Cập nhật thời gian đăng nhập cuối
                await db.ExecuteAsync("UPDATE Accounts SET LastLogin = GETDATE() WHERE AccountId = @Id", 
                    new { Id = user.AccountId });

                return new UserSessionDto
                {
                    AccountId = user.AccountId,
                    EmployeeId = user.EmployeeId,
                    Username = user.Username,
                    FullName = user.FullName,
                    RoleName = user.RoleName,
                    HotelName = user.HotelName
                };
            }

            return null;
        }

        /// <summary>
        /// Lấy thông tin hồ sơ chi tiết
        /// </summary>
        public async Task<UserProfileDto> GetProfileAsync(int employeeId)
        {
            using var db = CreateConnection();
            string sql = @"SELECT e.*, r.RoleName, a.Username
                           FROM Employees e
                           JOIN Roles r ON e.RoleId = r.RoleId
                           LEFT JOIN Accounts a ON e.EmployeeId = a.EmployeeId
                           WHERE e.EmployeeId = @Id";

            return await db.QueryFirstOrDefaultAsync<UserProfileDto>(sql, new { Id = employeeId });
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ (không bao gồm mật khẩu)
        /// </summary>
        public async Task<bool> UpdateProfileAsync(UserProfileDto profileDto)
        {
            using var db = CreateConnection();
            string sql = @"UPDATE Employees 
                           SET FullName = @FullName, 
                               Email = @Email, 
                               Phone = @Phone, 
                               Address = @Address, 
                               Gender = @Gender, 
                               DOB = @DOB
                           WHERE EmployeeId = @EmployeeId";

            int rows = await db.ExecuteAsync(sql, profileDto);
            return rows > 0;
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int employeeId, string currentPassword, string newPassword)
        {
            using var db = CreateConnection();
            var account = await db.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT AccountId, PasswordHash FROM Accounts WHERE EmployeeId = @Id", 
                new { Id = employeeId });

            if (account != null && BCrypt.Net.BCrypt.Verify(currentPassword, account.PasswordHash))
            {
                string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                int rows = await db.ExecuteAsync(
                    "UPDATE Accounts SET PasswordHash = @Hash WHERE AccountId = @Id", 
                    new { Hash = newHash, Id = account.AccountId });
                return rows > 0;
            }

            return false;
        }

        /// <summary>
        /// Tạo tài khoản lần đầu (Dùng cho setup)
        /// </summary>
        public async Task<bool> CreateInitialAccountAsync(int employeeId, string username, string password)
        {
            using var db = CreateConnection();
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            
            string sql = @"IF EXISTS (SELECT 1 FROM Accounts WHERE Username = @Username)
                           BEGIN
                               UPDATE Accounts SET PasswordHash = @Hash, EmployeeId = @EmployeeId WHERE Username = @Username
                           END
                           ELSE
                           BEGIN
                               INSERT INTO Accounts (EmployeeId, Username, PasswordHash) 
                               VALUES (@EmployeeId, @Username, @Hash)
                           END";

            int rows = await db.ExecuteAsync(sql, new { EmployeeId = employeeId, Username = username, Hash = hash });
            return rows > 0;
        }

        public async Task<bool> CreatePasswordResetRequestAsync(string username)
        {
            using var db = CreateConnection();
            
            // Lấy FullName từ Employee liên kết với Account
            string getInfoSql = @"SELECT e.FullName 
                                  FROM Accounts a
                                  JOIN Employees e ON a.EmployeeId = e.EmployeeId
                                  WHERE a.Username = @Username";
            
            var fullName = await db.QueryFirstOrDefaultAsync<string>(getInfoSql, new { Username = username });
            
            if (string.IsNullOrEmpty(fullName)) return false;

            string insertSql = "INSERT INTO PasswordResetRequests (Username, FullName) VALUES (@Username, @FullName)";
            int rows = await db.ExecuteAsync(insertSql, new { Username = username, FullName = fullName });
            
            return rows > 0;
        }
    }
}
