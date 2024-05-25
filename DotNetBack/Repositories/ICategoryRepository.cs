using DotNetBack.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetBack.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetUserCategoriesAsync(int user_id);
        Task<List<Category>> FindCategoriesAsync(string query);
        Task<int> AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int category_id);
        Task ResetProgressAsync(int category_id);
        Task ClearContentAsync(int category_id);
    }
}
