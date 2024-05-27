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
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {  
        //    }
        //}

        public async Task<IEnumerable<object>> GetCalendarAsync(int userId)
        {
            List<object> calendar = new List<object>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(@"
                WITH RepeatedWords AS (
                    SELECT
                        r.repetition_date,
                        COUNT(DISTINCT r.word_id) AS repeated_count
                    FROM
                        Repetition r
                    JOIN Word w ON r.word_id = w.word_id
                    JOIN Category c ON w.category_id = c.category_id
                    WHERE
                        c.user_id = @userId AND
                        r.repetition_date >= DATEADD(DAY, -7, GETDATE())
                    GROUP BY
                        r.repetition_date
                ),
                LearnedWords AS (
                    SELECT
                        r.repetition_date,
                        COUNT(DISTINCT r.word_id) AS learned_count
                    FROM
                        Repetition r
                    JOIN Word w ON r.word_id = w.word_id
                    JOIN Category c ON w.category_id = c.category_id
                    JOIN (
                        SELECT
                            word_id,
                            MIN(repetition_date) AS first_repetition_date
                        FROM
                            Repetition
                        GROUP BY
                            word_id
                    ) AS FirstRepetitions ON r.word_id = FirstRepetitions.word_id AND r.repetition_date = FirstRepetitions.first_repetition_date
                    WHERE
                        c.user_id = @userId AND
                        r.repetition_date >= DATEADD(DAY, -7, GETDATE())
                    GROUP BY
                        r.repetition_date
                )
                SELECT
                    COALESCE(rw.repetition_date, lw.repetition_date) AS repetition_date,
                    COALESCE(rw.repeated_count, 0) AS repeated_count,
                    COALESCE(lw.learned_count, 0) AS learned_count
                FROM
                    RepeatedWords rw
                FULL OUTER JOIN
                    LearnedWords lw ON rw.repetition_date = lw.repetition_date
                ORDER BY
                repetition_date;", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            calendar.Add(new
                            {
                                repetition_date = reader.GetDateTime(0),
                                repetition_num = reader.GetInt32(1),
                                learned_num = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
            return calendar;
        }



        public async Task<object> GetRecordAsync(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(@"
                    WITH UserRepetitions AS (
                        SELECT 
                            c.user_id,
                            r.word_id,
                            r.repetition_date,
                            LAG(r.repetition_date, 1) OVER (PARTITION BY c.user_id ORDER BY r.repetition_date) AS prev_date
                        FROM 
                            Word w
                        JOIN 
                            Repetition r ON w.word_id = r.word_id
                        JOIN 
                            Category c ON w.category_id = c.category_id
                        WHERE 
                            c.user_id = @UserId
                    ),
                    ConsecutiveDays AS (
                        SELECT
                            user_id,
                            repetition_date,
                            CASE 
                                WHEN DATEDIFF(day, prev_date, repetition_date) = 1 THEN 0
                                ELSE 1
                            END AS is_start_of_streak
                        FROM 
                            UserRepetitions
                    ),
                    Streaks AS (
                        SELECT
                            user_id,
                            repetition_date,
                            SUM(is_start_of_streak) OVER (PARTITION BY user_id ORDER BY repetition_date ROWS UNBOUNDED PRECEDING) AS streak_id
                        FROM 
                            ConsecutiveDays
                    ),
                    StreakLengths AS (
                        SELECT
                            user_id,
                            streak_id,
                            COUNT(*) AS streak_length,
                            MAX(repetition_date) AS streak_end_date
                        FROM 
                            Streaks
                        GROUP BY 
                            user_id, streak_id
                    )
                    SELECT 
                        MAX(streak_length) AS max_streak_length_ever,
                        MAX(CASE 
                            WHEN streak_end_date = CAST(GETDATE() AS DATE) THEN streak_length
                            ELSE 0
                        END) AS current_streak_length
                    FROM 
                        StreakLengths
                    WHERE 
                        user_id = @UserId;
                ", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new
                            {
                                consecutive_days = reader.GetInt32(1),
                                consecutive_days_record = reader.GetInt32(0)
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }


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
