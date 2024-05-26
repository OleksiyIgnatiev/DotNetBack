using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DotNetBack.Repositories;
using Microsoft.Extensions.Configuration;

namespace DotNetBack.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ppDBCon");
        }

        public async Task<bool> LoginAsync(string username, string passwordHash)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE username = @username AND password = @passwordHash", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);
                    int count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task RegisterAsync(string username, string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Calculate notification_time to be one day after registration
                DateTime notificationTime = DateTime.Now.AddDays(1);
                DateTime subscriptionPeriod = DateTime.Now;
                string notificationType = "email";
                string role = "user";

                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO Users (username, email, password, notification_time, level, subscription, subscription_period, notification_type, role) VALUES (@username, @Email, @Password, @NotificationTime, @Level, @Subscription, @SubscriptionPeriod, @NotificationType, @Role)",
                    connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@NotificationTime", notificationTime);
                    command.Parameters.AddWithValue("@Level", 0); // Adding level parameter with value 0
                    command.Parameters.AddWithValue("@Subscription", 0); // Adding subscription parameter with value 0
                    command.Parameters.AddWithValue("@SubscriptionPeriod", subscriptionPeriod); // Adding subscription_period with current date
                    command.Parameters.AddWithValue("@NotificationType", notificationType); // Adding notification_type with value "email"
                    command.Parameters.AddWithValue("@Role", role); // Adding role with value "user"

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task UpdateNotificationAsync(int userId, string notificationType, DateTime notificationTime)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("UPDATE Users SET notification_type = @notificationType, notification_time = @notificationTime WHERE user_id = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@notificationType", notificationType);
                    command.Parameters.AddWithValue("@notificationTime", notificationTime);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateUserAsync(int userId, string username, string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("UPDATE Users SET username = @username, email = @Email, password = @Password WHERE user_id = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateSubscriptionAsync(int userId, string subscription, DateTime subscriptionPeriod)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("UPDATE Users SET subscription = @subscription, subscription_period = @subscriptionPeriod WHERE user_id = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@subscription", subscription);
                    command.Parameters.AddWithValue("@subscriptionPeriod", subscriptionPeriod);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        //public async Task LogoutAsync(int userId) //TODO реализовать
        //{
        //}

        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            List<object> users = new List<object>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("SELECT user_id, username, email, password FROM Users", connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return users;
        }

        //public async Task<object> AdminActionAsync(int userId) //TODO реализовать
        //{
        //    return 1;
        //}

        public async Task DeleteUserAsync(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("DELETE FROM Users WHERE user_id = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        //public async Task<IEnumerable<object>> ShareSuccessesAsync(int userId)
        //{
        //    List<object> successes = new List<object>();
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();
        //        using (SqlCommand command = new SqlCommand("SELECT U.user_id, W.repetition_date " +
        //            "FROM Users U " +
        //            "JOIN Category C ON U.user_id = C.user_id " +
        //            "JOIN Word W ON C.category_id = W.category_id " +
        //            "WHERE U.user_id = @userId", connection))
        //        {
        //            command.Parameters.AddWithValue("@userId", userId);
        //            using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    successes.Add(new
        //                    {
        //                        UserId = reader.GetInt32(0),
        //                        RepetitionDate = reader.GetDateTime(1)
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return successes;
        //}

        //public async Task<IEnumerable<object>> GetCalendarAsync(int userId)
        //{
        //    List<object> calendar = new List<object>();
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();
        //        using (SqlCommand command = new SqlCommand("SELECT W.repetition_date, W.repetition_num " +
        //            "FROM Users U " +
        //            "JOIN Category C ON U.user_id = C.user_id " +
        //            "JOIN Word W ON C.category_id = W.category_id " +
        //            "WHERE U.user_id = @userId " +
        //            "AND W.repetition_date BETWEEN DATEADD(day, -7, GETDATE()) AND GETDATE()", connection))
        //        {
        //            command.Parameters.AddWithValue("@userId", userId);
        //            using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    calendar.Add(new
        //                    {
        //                        RepetitionDate = reader.GetDateTime(0),
        //                        RepetitionNum = reader.GetInt32(1)
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return calendar;
        //}

        //public async Task<object> GetRecordAsync(int userId) //TODO реализовать
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
               
        //    }
        //    return null;
        //}

        public async Task<object> GetUserInfoAsync(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("SELECT username, email, level, subscription, subscription_period, notification_type, notification_time FROM Users WHERE user_id = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new
                            {
                                Username = reader.GetString(0),
                                Email = reader.GetString(1),
                                Level = reader.GetInt32(2),
                                Subscription = reader.GetString(3),
                                SubscriptionPeriod = reader.GetDateTime(4),
                                NotificationType = reader.GetString(5),
                                NotificationTime = reader.GetTimeSpan(6)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
