using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelCore.Application.DTOs;
using HotelCore.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HotelCore.Infrastructure.Services
{
    public class SettingService : ISettingService
    {
        private readonly string _connectionString;

        public SettingService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<SystemSettingsDto> GetSystemSettingsAsync()
        {
            using var db = CreateConnection();
            var settings = await db.QueryAsync<dynamic>("SELECT SettingKey, SettingValue FROM Settings");
            var dict = settings.ToDictionary(s => (string)s.SettingKey, s => (string)s.SettingValue);

            return new SystemSettingsDto
            {
                HotelName = dict.GetValueOrDefault("HotelName", "HotelCore"),
                HotelAddress = dict.GetValueOrDefault("HotelAddress", ""),
                HotelPhone = dict.GetValueOrDefault("HotelPhone", ""),
                HotelEmail = dict.GetValueOrDefault("HotelEmail", ""),
                VatRate = decimal.Parse(dict.GetValueOrDefault("VatRate", "10")),
                Currency = dict.GetValueOrDefault("Currency", "VND"),
                ServiceFee = decimal.Parse(dict.GetValueOrDefault("ServiceFee", "0")),
                LogoPath = dict.GetValueOrDefault("LogoPath", "/images/logo.png")
            };
        }

        public async Task<bool> UpdateSystemSettingsAsync(SystemSettingsDto settingsDto)
        {
            using var db = CreateConnection();
            db.Open();
            using var trans = db.BeginTransaction();

            try
            {
                var updates = new Dictionary<string, string>
                {
                    { "HotelName", settingsDto.HotelName },
                    { "HotelAddress", settingsDto.HotelAddress },
                    { "HotelPhone", settingsDto.HotelPhone },
                    { "HotelEmail", settingsDto.HotelEmail },
                    { "VatRate", settingsDto.VatRate.ToString() },
                    { "Currency", settingsDto.Currency },
                    { "ServiceFee", settingsDto.ServiceFee.ToString() }
                };

                if (!string.IsNullOrEmpty(settingsDto.LogoPath))
                {
                    updates.Add("LogoPath", settingsDto.LogoPath);
                }

                foreach (var update in updates)
                {
                    await db.ExecuteAsync(
                        "UPDATE Settings SET SettingValue = @Value WHERE SettingKey = @Key",
                        new { Value = update.Value, Key = update.Key },
                        trans);
                }

                trans.Commit();
                return true;
            }
            catch
            {
                trans.Rollback();
                return false;
            }
        }

        public async Task<string> GetSettingValueAsync(string key)
        {
            using var db = CreateConnection();
            return await db.QueryFirstOrDefaultAsync<string>(
                "SELECT SettingValue FROM Settings WHERE SettingKey = @Key",
                new { Key = key });
        }
    }

    public static class DictionaryExtensions
    {
        public static string GetValueOrDefault(this Dictionary<string, string> dict, string key, string defaultValue)
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
