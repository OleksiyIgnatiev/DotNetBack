
using Microsoft.AspNetCore.Mvc;
using System.Data;
using DotNetBack.Models;
using DotNetBack.DataBase;

namespace DotNetBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class categoryController : Controller
    {
        public readonly IConfiguration _configuration;
        public categoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("add")]
        [HttpPost]
        public Response InsertCategory(string categoryName, int userId)
        {
            Response response = new Response();

            int iResult = 0;
            try
            {
                iResult = DBCategory.InsertCategory(_configuration, categoryName, userId);

                if (iResult > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Success";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Failed";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "Internal Server Error";
            }
            return response;
        }

        [Route("update")]
        [HttpPut]
        public Response UpdateCategory(int categoryId, string categoryName, int userId)
        {
            Response response = new Response();

            int iResult = 0;
            try
            {
                iResult = DBCategory.UpdateCategory(_configuration, categoryId, categoryName, userId);

                if (iResult > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Success";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Failed";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "Internal Server Error";
            }
            return response;
        }

        [Route("delete/{id}")]
        [HttpDelete]
        public Response DeleteCategory(int id)
        {
            Response response = new Response();

            int iResult = 0;
            try
            {
                iResult = DBCategory.DeleteCategory(_configuration, id);

                if (iResult > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Success";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Failed";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "Internal Server Error";
            }
            return response;
        }

        [HttpGet]
        [Route("get")]
        public List<Category> GetCategories()
        {
            DataTable ldt = DBCategory.GetCategoryDetails(_configuration);
            List<Category> categoryList = new List<Category>();

            if (ldt.Rows.Count > 0)
            {
                categoryList = (from DataRow dr in ldt.Rows
                                select new Category()
                                {
                                    CategoryId = Convert.ToInt32(dr["category_id"]),
                                    CategoryName = dr["category_name"].ToString(),
                                    UserId = Convert.ToInt32(dr["user_id"]),
                                }).ToList();
            }
            else
            {
                categoryList.Add(new Category
                {
                    CategoryName = "Failed"
                });
            }
            return categoryList;
        }

        [HttpGet]
        [Route("find/{query}")]
        public IActionResult FindCategories(string query)
        {
            DataTable dt = DBCategory.FindCategories(_configuration, query);
            if (dt == null || dt.Rows.Count == 0)
            {
                return NotFound(new { Status = "No categories found" });
            }

            var categories = (from DataRow dr in dt.Rows
                              select new
                              {
                                  category_name = dr["category_name"].ToString(),
                                  category_length = Convert.ToInt32(dr["category_length"]),
                                  progression_percentage = Convert.ToDouble(dr["progression_percentage"])
                              }).ToList();

            return Ok(categories);
        }

        [HttpGet]
        [Route("user/{userId}")]
        public IActionResult GetUserCategories(int userId)
        {
            DataTable dt = DBCategory.GetUserCategories(_configuration, userId);
            if (dt == null || dt.Rows.Count == 0)
            {
                return NotFound(new { Status = "No categories found for this user" });
            }

            var categories = (from DataRow dr in dt.Rows
                              select new
                              {
                                  category_name = dr["category_name"].ToString(),
                                  category_length = Convert.ToInt32(dr["category_length"]),
                                  progression_percentage = Convert.ToDouble(dr["progression_percentage"])
                              }).ToList();

            return Ok(categories);
        }

        [HttpPut]
        [Route("reset-progress/{categoryId}")]
        public Response ResetCategoryProgress(int categoryId)
        {
            Response response = new Response();

            try
            {
                int rowsAffected = DBCategory.ResetCategoryProgress(_configuration, categoryId);
                if (rowsAffected >= 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Category progress reset successfully";
                }
                else
                {
                    response.StatusCode = 500;
                    response.StatusMessage = "Failed to reset category progress";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "Internal Server Error";
            }
            return response;
        }

    }
}
