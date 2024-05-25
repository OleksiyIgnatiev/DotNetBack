using System.Collections.Generic;
using System.Data;
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
            var categories = new List<Category>();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Category WHERE User_Id = @UserId";
                    command.Parameters.AddWithValue("@UserId", user_id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var category = new Category
                            {
                                CategoryId = reader.GetInt32(reader.GetOrdinal("Category_Id")),
                                CategoryName = reader.GetString(reader.GetOrdinal("Category_Name")),
                                UserId = reader.GetInt32(reader.GetOrdinal("User_Id"))
                            };
                            categories.Add(category);
                        }
                    }
                }
            }

            return categories;
        }

        public async Task<List<Category>> FindCategoriesAsync(string query)
        {
            var categories = new List<Category>();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Category WHERE Category_Name LIKE @Query";
                    command.Parameters.AddWithValue("@Query", "%" + query + "%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var category = new Category
                            {
                                CategoryId = reader.GetInt32(reader.GetOrdinal("Category_Id")),
                                CategoryName = reader.GetString(reader.GetOrdinal("Category_Name")),
                                UserId = reader.GetInt32(reader.GetOrdinal("User_Id"))
                            };
                            categories.Add(category);
                        }
                    }
                }
            }

            return categories;
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Category (Category_Name, User_Id) VALUES (@CategoryName, @UserId); SELECT SCOPE_IDENTITY();";
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
                    command.CommandText = "UPDATE Category SET Category_Name = @CategoryName WHERE Category_Id = @CategoryId";
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
                    command.CommandText = "DELETE FROM Category WHERE Category_Id = @CategoryId";
                    command.Parameters.AddWithValue("@CategoryId", category_id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ResetProgressAsync(int category_id)
        {
            // Реализация сброса прогресса изучения слов в категории
        }

        public async Task ClearContentAsync(int category_id)
        {
            // Реализация очистки контента категории
        }
    }
}
