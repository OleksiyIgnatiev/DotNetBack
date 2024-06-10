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
    public class wordsController : ControllerBase
    {
        //string connectionString = "Data Source= localhost;Initial Catalog=VocDB;User ID=sa;Password=12345678;Encrypt=False";

        private readonly IWordRepository wordRepository;

        public wordsController(IWordRepository wordRepository)
        {
            this.wordRepository = wordRepository;
        }

        [HttpGet("{category_id}")]
        public async Task<IActionResult> GetUserCategories(int category_id)
        {
            Response words = await wordRepository.GetWordsAsync(category_id);
            if (words.StatusCode == 200)
            {
                return Ok(words);
            }
            return StatusCode(words.StatusCode, words.Message);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWord([FromBody] Word word)
        {
            if (word == null)
            {
                return BadRequest("Word is null.");
            }

            Response response = await wordRepository.AddWordAsync(word);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpDelete("{word_id}")]
        public async Task<IActionResult> DeleteWord(int word_id)
        {
            Response response = await wordRepository.DeleteWordAsync(word_id);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWord(Word word)
        {
            Response response = await wordRepository.UpdateWordAsync(word);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpGet("image/{wordName}")]
        public async Task<IActionResult> GenerateImage(string wordName)
        {
            string imageUrl = await wordRepository.GetImageUrlAsync(wordName);

            if (imageUrl != null)
            {
                // Вернуть URL изображения в виде ответа
                return Ok(imageUrl);
            }
            else
            {
                // В случае ошибки или отсутствия изображения вернуть сообщение об ошибке
                return NotFound("Изображение для данного слова не найдено.");
            }
        }

        // PUT: api/word/relized/{wordId}
        [HttpPut("realized/{wordId}")]
        public async Task<IActionResult> RealizeWord(int wordId)
        {
            // Вызов метода из репозитория
            Response response = await wordRepository.RealizeWordAsync(wordId);

            // Обработка ответа
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response.Message);
            }
        }
    }
}

// проверка гит