using DotNetBack.Models;
using DotNetBack.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;


namespace DotNetBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordController : ControllerBase
    {

        private readonly IWordRepository wordRepository;

        public WordController(IWordRepository wordRepository)
        {
            this.wordRepository = wordRepository;
        }

        [HttpGet("word/{category_id}")]
        public async Task<IActionResult> GetUserCategories(int category_id)
        {
            List<Word> words = await wordRepository.GetWordsAsync(category_id);
            return Ok(words);
        }


        [HttpDelete]
        [Route("DeleteWord")]
        public JsonResult DeleteWord(int id)
        {
            return null;
        }
    }
}

// проверка гит