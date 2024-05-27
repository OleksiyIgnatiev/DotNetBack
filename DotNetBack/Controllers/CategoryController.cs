using Microsoft.AspNetCore.Mvc;
using System.Data;
using DotNetBack.Models;
using DotNetBack.Repositories;

namespace DotNetBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class сategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public сategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: /api/category/user/:user_id
        [HttpGet("{user_id}")]
        public async Task<IActionResult> GetUserCategories(int user_id)
        {
            var categories = await _categoryRepository.GetUserCategoriesAsync(user_id);
            return Ok(new Response(200, "Categories retrieved successfully.", categories));
        }

        // GET: /api/category/find/:query
        [HttpGet("find/{query}")]
        public async Task<IActionResult> FindCategories(string query, [FromQuery] int user_id)
        {
            var categories = await _categoryRepository.FindCategoriesAsync(query, user_id);
            return Ok(new Response(200, "Categories found successfully.", categories));
        }

        // POST: /api/category
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest(new Response(400, "Category is null."));
            }

            var categoryId = await _categoryRepository.AddCategoryAsync(category);
            return Ok(new Response(201, "Category created successfully.", new { CategoryId = categoryId }));
        }

        // PUT: /api/category
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest(new Response(400, "Category is null."));
            }

            await _categoryRepository.UpdateCategoryAsync(category);
            return Ok(new Response(200, "Category updated successfully."));
        }

        // DELETE: /api/category
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int category_id)
        {
            await _categoryRepository.DeleteCategoryAsync(category_id);
            return Ok(new Response(200, "Category deleted successfully."));
        }

        // PUT: /api/category/reset-progress/:category_id
        [HttpPut("reset-progress/{category_id}")]
        public async Task<IActionResult> ResetProgress(int category_id)
        {
            await _categoryRepository.ResetProgressAsync(category_id);
            return Ok(new Response(200, "Category progress reset successfully."));
        }

        // PUT: /api/category/clear-content/:category_id
        [HttpPut("clear-content/{category_id}")]
        public async Task<IActionResult> ClearContent(int category_id)
        {
            await _categoryRepository.ClearContentAsync(category_id);
            return Ok(new Response(200, "Category content cleared successfully."));
        }
    }
}