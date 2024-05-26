﻿using DotNetBack.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetBack.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<CategoryInfo>> GetUserCategoriesAsync(int user_id);
        Task<List<CategoryInfo>> FindCategoriesAsync(string query, int user_id);
        Task<int> AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int category_id);
        Task ResetProgressAsync(int category_id);
        Task ClearContentAsync(int category_id);
    }
}
