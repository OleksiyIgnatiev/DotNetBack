using Microsoft.AspNetCore.Mvc;
using System.Data;
using DotNetBack.Models;
using DotNetBack.Repositories;

namespace DotNetBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: /api/category/user/:user_id
        [HttpGet("user/{user_id}")]
        public async Task<IActionResult> GetUserCategories(int user_id)
        {
            var categories = await _categoryRepository.GetUserCategoriesAsync(user_id);
            if (categories.StatusCode == 200) 
            {
                return Ok(categories);
            }
            return StatusCode(categories.StatusCode, categories.Message);
        }

        // GET: /api/category/find/:query
        [HttpGet("find/{query}")]
        public async Task<IActionResult> FindCategories(string query, [FromQuery] int user_id)
        {
            var categories = await _categoryRepository.FindCategoriesAsync(query, user_id);
            if (categories.StatusCode == 200)
            {
                return Ok(categories);
            }
            return StatusCode(categories.StatusCode, categories.Message);
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
            if (categoryId.StatusCode == 200)
            {
                return Ok(categoryId);
            }
            return StatusCode(categoryId.StatusCode, categoryId.Message);
        }

        // PUT: /api/category
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest(new Response(400, "Category is null."));
            }

            var response = await _categoryRepository.UpdateCategoryAsync(category);
            if (response.StatusCode == 200)
            {
                return Ok(new Response(200, "Category updated successfully."));
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        // DELETE: /api/category
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int category_id)
        {
            var response = await _categoryRepository.DeleteCategoryAsync(category_id);
            if (response.StatusCode == 200)
            {
                return Ok("Category deleted successfully.");
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        // PUT: /api/category/reset-progress/:category_id
        [HttpPut("reset-progress/{category_id}")]
        public async Task<IActionResult> ResetProgress(int category_id)
        {
            var response = await _categoryRepository.ResetProgressAsync(category_id);
            if (response.StatusCode == 200)
            {
                return Ok("Category progress reset successfully.");
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        // PUT: /api/category/clear-content/:category_id
        [HttpPut("clear-content/{category_id}")]
        public async Task<IActionResult> ClearContent(int category_id)
        {
            var response = await _categoryRepository.ClearContentAsync(category_id);

            if (response.StatusCode == 200)
            {
                return Ok("Category content cleared successfully.");
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        
    }
}