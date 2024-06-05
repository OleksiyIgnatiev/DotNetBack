using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DotNetBack.Models;
using DotNetBack.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DotNetBack.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ppDBCon");
        }

        private static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            // Генерация соли
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 16, 1000))
            {
                byte[] salt = bytes.Salt;
                byte[] buffer2 = bytes.GetBytes(32);

                byte[] dst = new byte[49];
                Buffer.BlockCopy(salt, 0, dst, 1, 16);
                Buffer.BlockCopy(buffer2, 0, dst, 17, 32);
                return Convert.ToBase64String(dst);
            }
        }

        private static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] src = Convert.FromBase64String(hashedPassword);
            if (src.Length != 49 || src[0] != 0)
            {
                return false;
            }

            byte[] salt = new byte[16];
            Buffer.BlockCopy(src, 1, salt, 0, 16);
            byte[] storedHash = new byte[32];
            Buffer.BlockCopy(src, 17, storedHash, 0, 32);

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, salt, 1000))
            {
                byte[] computedHash = bytes.GetBytes(32);
                return ByteArraysEqual(storedHash, computedHash);
            }
        }

        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<Response> LoginAsync(string username, string passwordHash)
        {
            Response response = new Response();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand("SELECT Count(*) FROM Users WHERE username = @username", connection);
                    command.Parameters.AddWithValue("@username", username);

                    int result = (int)await command.ExecuteScalarAsync();

                    if (result == 0)
                    {
                        response.Data = null;
                        response.StatusCode = 500;
                        response.Message = "This username doesn`t exist";
                        return response;
                    }

                    using (command = new SqlCommand("SELECT password, user_id, role FROM Users WHERE username = @username", connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string password = reader["password"].ToString();
                                int userId = Convert.ToInt32(reader["user_id"]);
                                string role = reader["role"].ToString();

                                if (!VerifyHashedPassword(password, passwordHash))
                                {
                                    response.Data = null;
                                    response.StatusCode = 500;
                                    response.Message = "Wrong password";
                                    return response;
                                }

                                response.Data = new Login() { userId = userId, role = role };

                                return response;
                            }
                            else
                            {
                                // Handle case where no user is found
                                response.Data = null;
                                return response;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> RegisterAsync(string username, string email, string password)
        {
            Response response = new Response();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    List<int> list = new List<int>();

                    // Calculate notification_time to be one day after registration
                    DateTime notificationTime = DateTime.Now.AddDays(1);
                    DateTime subscriptionPeriod = DateTime.Now;
                    string notificationType = "email";
                    string role = "user";

                    // Begin transaction
                    SqlTransaction transaction = connection.BeginTransaction();


                    SqlCommand command = new SqlCommand($"select Count(*) from Users where username = '{username}'",
                        connection, transaction);

                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                    if (count != 0)
                    {
                        response.StatusCode = 500;
                        response.Message = "This username is already picked";
                        return response;
                    }


                    command = new SqlCommand($"select Count(*) from Users where email = '{email}'",
                        connection, transaction);

                    count = Convert.ToInt32(await command.ExecuteScalarAsync());

                    if (count != 0)
                    {
                        response.StatusCode = 500;
                        response.Message = "This email is already picked";
                        return response;
                    }

                    // Insert user into Users table
                    using (command = new SqlCommand(
                        "INSERT INTO Users (username, email, password, notification_time, level, subscription, subscription_period, notification_type, role) " +
                        "VALUES (@username, @Email, @Password, @NotificationTime, @Level, @Subscription, @SubscriptionPeriod, @NotificationType, @Role); " +
                        "SELECT SCOPE_IDENTITY();",
                        connection, transaction))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", HashPassword(password));
                        command.Parameters.AddWithValue("@NotificationTime", notificationTime);
                        command.Parameters.AddWithValue("@Level", 0);
                        command.Parameters.AddWithValue("@Subscription", 0);
                        command.Parameters.AddWithValue("@SubscriptionPeriod", subscriptionPeriod);
                        command.Parameters.AddWithValue("@NotificationType", notificationType);
                        command.Parameters.AddWithValue("@Role", role);

                        // Execute scalar to get the newly inserted user_id
                        int userId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        // Read JSON file content
                        string jsonFilePath = "TemplateData.json";
                        string jsonContent = await File.ReadAllTextAsync(jsonFilePath);

                        // Deserialize JSON to object
                        var data = JsonConvert.DeserializeObject<Data>(jsonContent);

                        // Insert categories associated with the user
                        foreach (var category in data.Categories)
                        {
                            try
                            {

                                SqlCommand CatCommand = new SqlCommand("INSERT INTO Category (category_name, user_id) " +
                                    "VALUES (@CategoryName, @UserId); SELECT SCOPE_IDENTITY();", connection, transaction);

                                CatCommand.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                                CatCommand.Parameters.AddWithValue("@UserId", userId);

                                int CategoryId = Convert.ToInt32(await CatCommand.ExecuteScalarAsync());

                                list.Add(CategoryId);
                            }
                            catch (Exception ex)
                            {
                                response.StatusCode = 500;
                                response.Message = ex.Message;
                            }
                        }

                        foreach (var word in data.Words)
                        {
                            int categoryId = list[word.CategoryId - 1]; // Consider null categoryId
                            using (SqlCommand addWordCommand = new SqlCommand(
                                "INSERT INTO Word (name, translation, category_id, img_link, repetition_num) " +
                                "VALUES (@name, @translation, @categoryId, @imgLink, @repetitionNum)",
                                connection, transaction))
                            {
                                addWordCommand.Parameters.AddWithValue("@name", word.Name);
                                addWordCommand.Parameters.AddWithValue("@translation", word.Translation);
                                addWordCommand.Parameters.AddWithValue("@categoryId", categoryId);
                                addWordCommand.Parameters.AddWithValue("@imgLink", word.ImgLink);
                                addWordCommand.Parameters.AddWithValue("@repetitionNum", word.RepetitionNum);
                                await addWordCommand.ExecuteNonQueryAsync();
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }


        public class Data
        {
            public List<Category> Categories { get; set; }
            public List<Word> Words { get; set; }
        }




        public async Task<Response> UpdateNotificationAsync(int userId, string notificationType, DateTime notificationTime)
        {
            Response response = new Response();
            try
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
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> UpdateUserAsync(int userId, string username, string email, string password)
        {
            Response response = new Response();
            try
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
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> UpdateSubscriptionAsync(int userId, string subscription, DateTime subscriptionPeriod)
        {
            Response response = new Response();
            try
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
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        //public async Task LogoutAsync(int userId) //TODO реализовать
        //{
        //}

        public async Task<Response> GetAllUsersAsync()
        {
            Response response = new Response();
            try
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
                            response.Data = users;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        //public async Task<object> AdminActionAsync(int userId) //TODO реализовать
        //{
        //}

        public async Task<Response> DeleteUserAsync(int userId)
        {
            Response response = new Response();
            try
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
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        //public async Task<IEnumerable<object>> ShareSuccessesAsync(int userId)
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {  
        //    }
        //}

        public async Task<Response> GetCalendarAsync(int userId)
        {
            Response response = new Response();
            try
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
                response.Data = calendar;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }



        public async Task<Response> GetRecordAsync(int userId)
        {
            Response response = new Response();
            try
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
                                response.Data = new
                                {
                                    consecutive_days = reader.GetInt32(1),
                                    consecutive_days_record = reader.GetInt32(0)
                                };
                            }
                            else
                            {
                                response.Data = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }



        public async Task<Response> GetUserInfoAsync(int userId)
        {
            Response response = new Response();
            try
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
                                // Отримання значень з рядка результатів і заповнення об'єкта Response
                                response.StatusCode = 200; // Успішний статус код
                                response.Message = "User info retrieved successfully.";

                                // Зчитування значень з рядка результатів
                                response.Data = new
                                {
                                    Username = reader["username"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Level = Convert.ToInt32(reader["level"]),
                                    Subscription = reader["subscription"].ToString(),
                                    SubscriptionPeriod = reader["subscription_period"].ToString(),
                                    NotificationType = reader["notification_type"].ToString(),
                                    NotificationTime = reader["notification_time"].ToString()
                                };
                            }
                            else
                            {
                                // Якщо користувача з вказаним ID не знайдено
                                response.StatusCode = 404; // Статус код для "Не знайдено"
                                response.Message = "User not found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обробка помилок
                response.StatusCode = 500; // Статус код для "Помилка сервера"
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<Response> GetUserByEmailAsync(string email)
        {
            Response response = new Response();
            try
            {
                string query = "SELECT id, role FROM Users WHERE email = @Email";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Отримання значень з рядка результатів і заповнення об'єкта Response
                                response.StatusCode = 200; // Успішний статус код
                                response.Message = "User info retrieved successfully.";

                                // Зчитування значень з рядка результатів
                                response.Data = new
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Role = reader["role"].ToString()
                                };
                            }
                            else
                            {
                                // Якщо користувача з вказаною електронною поштою не знайдено
                                response.StatusCode = 404; // Статус код для "Не знайдено"
                                response.Message = "User not found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обробка помилок
                response.StatusCode = 500; // Статус код для "Помилка сервера"
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
