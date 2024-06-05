using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DotNetBack.Models;
using Microsoft.Extensions.Configuration;

namespace DotNetBack.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IConfiguration _configuration;

        public CategoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("ppDBCon"));
        }

        public async Task<List<Category>> GetUserCategoriesAsync(int user_id)
        {
            var categoriesProgress = new List<Category>();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    SELECT 
                        c.category_name,
                        COUNT(w.word_id) AS category_length,
                        COALESCE(SUM(CASE WHEN w.repetition_num > 20 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(w.word_id), 0), 0) AS progression_percentage
                    FROM Category c
                    LEFT JOIN Word w ON c.category_id = w.category_id
                    WHERE c.user_id = @UserId
                    GROUP BY c.category_id, c.category_name";

                    command.Parameters.AddWithValue("@UserId", user_id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var categoryProgress = new Category
                            {
                                CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                                CategoryLength = reader.GetInt32(reader.GetOrdinal("category_length")),
                                ProgressionPercentage = reader.IsDBNull(reader.GetOrdinal("progression_percentage"))
                                                        ? 0
                                                        : (double)reader.GetDecimal(reader.GetOrdinal("progression_percentage"))
                            };
                            categoriesProgress.Add(categoryProgress);
                        }
                    }
                }
            }

            return categoriesProgress;
        }

        public async Task<List<Category>> FindCategoriesAsync(string query, int userId)
        {
            var categoriesProgress = new List<Category>();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
            SELECT 
                c.category_name,
                COUNT(w.word_id) AS category_length,
                COALESCE(SUM(CASE WHEN w.repetition_num > 20 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(w.word_id), 0), 0) AS progression_percentage
            FROM Category c
            LEFT JOIN Word w ON c.category_id = w.category_id
            WHERE c.category_name LIKE @Query
            AND c.user_id = @UserId
            GROUP BY c.category_id, c.category_name";

                    command.Parameters.AddWithValue("@Query", "%" + query + "%");
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var categoryProgress = new Category
                            {
                                CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                                CategoryLength = reader.GetInt32(reader.GetOrdinal("category_length")),
                                ProgressionPercentage = reader.IsDBNull(reader.GetOrdinal("progression_percentage"))
                                                        ? 0
                                                        : (double)reader.GetDecimal(reader.GetOrdinal("progression_percentage"))
                            };
                            categoriesProgress.Add(categoryProgress);
                        }
                    }
                }
            }

            return categoriesProgress;
        }


        public async Task<int> AddCategoryAsync(Category category)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Category (category_name, user_id) VALUES (@CategoryName, @UserId); SELECT SCOPE_IDENTITY();";
                    command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    command.Parameters.AddWithValue("@UserId", category.UserId);

                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Category SET category_name = @CategoryName WHERE category_id = @CategoryId";
                    command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    command.Parameters.AddWithValue("@CategoryId", category.CategoryId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteCategoryAsync(int category_id)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Category WHERE category_id = @CategoryId";
                    command.Parameters.AddWithValue("@CategoryId", category_id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ResetProgressAsync(int category_id)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    UPDATE Word 
                    SET 
                        repetition_num = 0,
                        repetition_date = GETDATE()
                    WHERE 
                        category_id = @CategoryId";
                    command.Parameters.AddWithValue("@CategoryId", category_id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ClearContentAsync(int category_id)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    DELETE FROM Word 
                    WHERE 
                        category_id = @CategoryId";
                    command.Parameters.AddWithValue("@CategoryId", category_id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}