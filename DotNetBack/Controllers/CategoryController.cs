using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest("Category is null.");
            }

            var categoryId = await _categoryRepository.AddCategoryAsync(category);
            return Ok(new { CategoryId = categoryId });
        }
    }
}
