using DotNetBack.Models;

namespace DotNetBack.Repositories
{
    public interface ICategoryRepository
    {
        Task<int> AddCategoryAsync(Category category);
    }
}
