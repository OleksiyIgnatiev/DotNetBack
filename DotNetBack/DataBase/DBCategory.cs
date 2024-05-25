using System.Data;
using System.Data.SqlClient;
using DotNetBack.Repositories;
using Microsoft.Extensions.Configuration;

namespace DotNetBack.DataBase
{
    public static class DBCategory
    {
        public static int InsertCategory(IConfiguration _configuration, string categoryName, int userId)
        {
            int output = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
                {
                    string query = "INSERT INTO Category (category_name, user_id) VALUES (@category_name, @user_id)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@category_name", categoryName);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    output = 1;
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки или обработка при необходимости
            }
            return output;
        }

        public static int UpdateCategory(IConfiguration _configuration, int categoryId, string categoryName, int userId)
        {
            int output = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
                {
                    string query = "UPDATE Category SET category_name = @category_name, user_id = @user_id WHERE category_id = @category_id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@category_id", categoryId);
                    cmd.Parameters.AddWithValue("@category_name", categoryName);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    output = 1;
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки или обработка при необходимости
            }
            return output;
        }

        public static int DeleteCategory(IConfiguration _configuration, int categoryId)
        {
            int output = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
                {
                    string query = "DELETE FROM Category WHERE category_id = @category_id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@category_id", categoryId);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    output = 1;
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки или обработка при необходимости
            }
            return output;
        }

        public static DataTable GetCategoryDetails(IConfiguration _configuration)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
                {
                    string query = "SELECT category_id, category_name, user_id FROM Category";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки или обработка при необходимости
                return null;
            }
            return dt;
        }
        public static DataTable FindCategories(IConfiguration _configuration, string query)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
                {
                    string sqlQuery = @"
                        SELECT 
                            c.category_name, 
                            COUNT(w.word_id) AS category_length,
                            SUM(CASE 
                                WHEN w.repetition_num > 20 THEN 1
                                ELSE 0
                            END) * 100.0 / COUNT(w.word_id) AS progression_percentage
                        FROM Category c
                        LEFT JOIN Word w ON c.category_id = w.category_id
                        WHERE c.category_name LIKE '%' + @query + '%'
                        GROUP BY c.category_name";

                    SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@query", query);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки или обработка при необходимости
                return null;
            }
            return dt;
        }

        public static DataTable GetUserCategories(IConfiguration _configuration, int userId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
                {
                    string sqlQuery = @"
                        SELECT 
                            c.category_name, 
                            COUNT(w.word_id) AS category_length,
                            SUM(CASE 
                                WHEN w.repetition_num > 20 THEN 1
                                ELSE 0
                            END) * 100.0 / COUNT(w.word_id) AS progression_percentage
                        FROM Category c
                        LEFT JOIN Word w ON c.category_id = w.category_id
                        WHERE c.user_id = @userId
                        GROUP BY c.category_name";

                    SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@userId", userId);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки или обработка при необходимости
                return null;
            }
            return dt;
        }
        public static int ResetCategoryProgress(IConfiguration _configuration, int categoryId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
                {
                    string sqlQuery = @"UPDATE Word SET repetition_num = 0 WHERE category_id = @categoryId";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@categoryId", categoryId);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки или обработка при необходимости
                return -1;
            }
        }
    }
}
