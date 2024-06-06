using DotNetBack.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetBack.Repositories
{
    public interface ICategoryRepository
    {
        Task<Response> GetUserCategoriesAsync(int user_id);
        Task<Response> FindCategoriesAsync(string query, int user_id);
        Task<Response> AddCategoryAsync(Category category);
        Task<Response> UpdateCategoryAsync(Category category);
        Task<Response> DeleteCategoryAsync(int category_id);
        Task<Response> ResetProgressAsync(int category_id);
        Task<Response> ClearContentAsync(int category_id);
    }
}