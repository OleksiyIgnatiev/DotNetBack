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

        [HttpPost]
        public async Task<IActionResult> CreateWord([FromBody] Word word)
        {
            if (word == null)
            {
                return BadRequest("Word is null.");
            }

            var word_id = await wordRepository.AddWordAsync(word);
            return Ok(new { Word_Id = word_id });
        }

        [HttpDelete("word/{word_id}")]
        public async Task<IActionResult> DeleteWord(int word_id)
        {
            wordRepository.DeleteWordAsync(word_id);
            return Ok();
        }

        [HttpPut("word/{word_id}")]
        public async Task<IActionResult> UpdateWord(int word_id, Word word)
        {
            word.WordId = word_id;
            wordRepository.UpdateWordAsync(word);
            return Ok();
        }
    }
}

// проверка гит