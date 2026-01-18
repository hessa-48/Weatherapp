using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WEATHERAPP.Models;

namespace WEATHERAPP.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ===========================
        // 1️⃣ التحقق من تسجيل دخول المستخدم
        // ===========================
        public User? ValidateUser(string username, string password)
        {
            User? user = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_LoginUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            UserId = (int)reader["UserId"],
                            Username = reader["Username"] as string,
                            Password = reader["Password"] as string
                        };
                    }
                }
            }

            return user;
        }

        // ===========================
        // 2️⃣ إدراج نتيجة الطقس الجديدة
        // ===========================
        public void InsertWeatherResult(WeatherResult result)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_AddWeatherResult", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", result.UserId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", result.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Humidity", result.Humidity ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TempMin", result.TempMin ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TempMax", result.TempMax ?? (object)DBNull.Value);

                // ✅ هنا نمرر التاريخ المطلوب للـ SP
                cmd.Parameters.AddWithValue("@Date", result.DateRecorded);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ===========================
        // 3️⃣ جلب جميع نتائج الطقس لمستخدم معين
        // ===========================
        public List<WeatherResult> GetUserWeatherResults(int userId)
        {
            var results = new List<WeatherResult>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetUserWeatherResults", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new WeatherResult
                        {
                            ResultId = (int)reader["ResultId"],
                            UserId = reader["UserId"] as int?,
                            City = reader["City"] as string,
                            Humidity = reader["Humidity"] as int?,
                            TempMin = reader["TempMin"] as double?,
                            TempMax = reader["TempMax"] as double?,
                            DateAdded = reader["DateAdded"] as DateTime?,
                            DateRecorded = (DateTime)reader["DateRecorded"]
                        });
                    }
                }
            }

            return results;
        }

        // ===========================
        // 4️⃣ تحديث سجل الطقس الموجود
        // ===========================
        public void UpdateWeatherResult(WeatherResult result)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_UpdateWeatherResult", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ResultId", result.ResultId);
                cmd.Parameters.AddWithValue("@City", result.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Humidity", result.Humidity ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TempMin", result.TempMin ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TempMax", result.TempMax ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", result.DateRecorded);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
