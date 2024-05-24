using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using DotNetBack.Models;

namespace DotNetBack.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IConfiguration _configuration;

        public CategoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            var sql = "INSERT INTO Category (category_name, user_id) VALUES (@CategoryName, @UserId); SELECT CAST(SCOPE_IDENTITY() as int)";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
            {
                connection.Open();
                var result = await connection.QuerySingleAsync<int>(sql, new { category.CategoryName, category.UserId });
                return result;
            }
        }
    }
}
