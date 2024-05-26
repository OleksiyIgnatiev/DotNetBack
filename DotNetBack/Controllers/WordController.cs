using DotNetBack.Models;
using DotNetBack.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Data;
using System.Data.SqlClient;


namespace DotNetBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordController : ControllerBase
    {
        //string connectionString = "Data Source= localhost;Initial Catalog=VocDB;User ID=sa;Password=12345678;Encrypt=False";

        private readonly IWordRepository wordRepository;

        public WordController(IWordRepository wordRepository)
        {
            this.wordRepository = wordRepository;
        }

        [HttpGet("{category_id}")]
        public async Task<IActionResult> GetUserCategories(int category_id)
        {
            List<Word> words = await wordRepository.GetWordsAsync(category_id);
            return Ok(words);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWord([FromBody] Word word)
        {
            if (word == null)
            {
                return BadRequest("Word is null.");
            }

            Response response = await wordRepository.AddWordAsync(word);
            return Ok(response);
        }

        [HttpDelete("{word_id}")]
        public async Task<IActionResult> DeleteWord(int word_id)
        {
            Response response = await wordRepository.DeleteWordAsync(word_id);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWord(Word word)
        {
            Response response = await wordRepository.UpdateWordAsync(word);
            return Ok(response);
        }
    }
}

// проверка гит