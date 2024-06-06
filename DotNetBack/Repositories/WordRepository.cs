using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using DotNetBack.Models;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Azure;
using Response = DotNetBack.Models.Response;
using System.Net.Http;


namespace DotNetBack.Repositories
{
    public class WordRepository : IWordRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public WordRepository(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("ppDBCon"));
        }

        public async Task<Response> GetWordsAsync(int category_id)
        {
            Response response = new Response();

            List<Word> words = new List<Word>();
            response.Data = words;

            SqlConnection connection = GetConnection();

            try
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM Word WHERE category_id = @category_id";
                command.Parameters.AddWithValue("@category_id", category_id);

                var commandCheck = connection.CreateCommand();
                commandCheck.CommandText = $"Select Count(*) FROM Word WHERE category_id = {category_id}";
                int result = (int)await commandCheck.ExecuteScalarAsync();

                if (result == 0)
                {

                    response.StatusCode = 500;
                    response.Message = "There are no words in this category";
                    return response;
                }


                var reader = await command.ExecuteReaderAsync();


                while (await reader.ReadAsync())
                {
                    int word_id = Convert.ToInt32(reader["word_id"]);
                    string name = reader["name"].ToString();
                    string translation = reader["translation"].ToString();
                    string img_link = reader["img_link"].ToString();
                    int repetition_num = Convert.ToInt32(reader["repetition_num"]);

                    Word word = Word.Create(word_id, name, translation, category_id, img_link, repetition_num);

                    words.Add(word);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<Response> DeleteWordAsync(int wordId)
        {
            Response response = new Response();

            string sql = $"DELETE FROM Word WHERE word_id = {wordId}";

            var connection = GetConnection();
            try
            {

                var command = connection.CreateCommand();


                command.CommandText = sql;

                await connection.OpenAsync();

                var commandCheck = connection.CreateCommand();
                commandCheck.CommandText = $"Select Count(*) FROM Word WHERE word_id = {wordId}";
                int result = (int)await commandCheck.ExecuteScalarAsync();

                if (result == 0)
                {
                    response.StatusCode = 500;
                    response.Message = "Such word does *not exist";
                    return response;
                }

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;

        }

        public async Task<Response> UpdateWordAsync(Word word)
        {
            Response response = new Response();
            string sql = "UPDATE Word SET " +
                         $"name = '{word.Name}', " +
                         $"translation = '{word.Translation}', " +
                         $"category_id = {word.CategoryId}, " +
                         $"img_link = '{word.ImgLink}', " +
                         $"repetition_num = {word.RepetitionNum} " +
                         $"WHERE word_id = {word.WordId}";

            var connection = GetConnection();
            try
            {
                var command = connection.CreateCommand();
                command.CommandText = sql;

                await connection.OpenAsync();

                SqlCommand commandCheck = connection.CreateCommand();
                commandCheck.CommandText = $"Select Count(*) FROM Word WHERE word_id = {word.WordId}";
                int result = (int)await commandCheck.ExecuteScalarAsync();

                if (result == 0)
                {
                    response.StatusCode = 500;
                    response.Message = "Such word does not exist";
                    return response;
                }

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> AddWordAsync(Word word)
        {
            Response response = new Response();
            var connection = GetConnection();

            try
            {
                await connection.OpenAsync();

                string imageUrl = null;

                // Check if the image path is provided
                if (!string.IsNullOrEmpty(word.ImgLink) && !word.ImgLink.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    // If the ImgLink is provided and not equal to "string", assume it's a local path
                    string localImagePath = word.ImgLink;

                    // Copy the image to the destination folder
                    string destinationFolder = Path.Combine(Directory.GetCurrentDirectory(), "Services", "Images");
                    string fileName = Path.GetFileName(localImagePath);
                    string destinationPath = Path.Combine(destinationFolder, fileName);

                    File.Copy(localImagePath, destinationPath, true);

                    // Update imageUrl to point to the copied image
                    imageUrl = $"~/Services/Images/{fileName}";
                }
                else
                {
                    // Fetch image URL from Google if ImgLink is not provided or is "string"
                    imageUrl = await GetImageUrlAsync(word.Name);
                    if (imageUrl == null)
                    {
                        response.StatusCode = 500;
                        response.Message = "Failed to fetch image URL from Google.";
                        return response;
                    }
                }

                var command = connection.CreateCommand();

                command.CommandText = "INSERT INTO Word (name, translation, category_id, img_link, repetition_num)" +
                    $"VALUES ('{word.Name}', '{word.Translation}', {word.CategoryId}, '{imageUrl}', {word.RepetitionNum}); SELECT SCOPE_IDENTITY();";

                response.Data = Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                response.Data = 0;
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return response;
        }


        public async Task<string> GetImageUrlAsync(string wordName)
        {
            var apiKey = _configuration["GoogleCustomSearch:ApiKey"];
            var searchEngineId = _configuration["GoogleCustomSearch:SearchEngineId"];

            var query = $"{wordName} image";
            var url = $"https://www.googleapis.com/customsearch/v1?q={query}&cx={searchEngineId}&key={apiKey}&searchType=image";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                string imageUrl = data.items[0].link;
                return imageUrl;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error fetching image URL: {ex.Message}");
                return null;
            }
        }
    }
}
//Смотри ниже Я приведу метод который на основе названия нашего слова генерирует ссылку Google картинки и вставляют в свойства Image Link в нашей базе данных Мне нужно чтобы ты переделывал немного этот метод чтобы мы туда писали путь к нашей картинке который находится у нас в проводнике оно загружало его в папку Services/Images нашего проекта и после вставил сылку на картинку в наше свойство Image Link. А в случае если он ничего не загрузил и нам передано NULL или "string" Тогда нужно использовать метод который мы уже имеем с генерацией ссылки Google 

//public async Task<Response> AddWordAsync(Word word)
//{
//    Response response = new Response();
//    var connection = GetConnection();

//    try
//    {
//        await connection.OpenAsync();

//        // Fetch image URL from Google
//        string imageUrl = await GetImageUrlAsync(word.Name);
//        if (imageUrl == null)
//        {
//            response.StatusCode = 500;
//            response.Message = "Failed to fetch image URL from Google.";
//            return response;
//        }

//        var command = connection.CreateCommand();

//        command.CommandText = "INSERT INTO Word (name, translation, category_id, img_link, repetition_num)" +
//            $"VALUES ('{word.Name}', '{word.Translation}', {word.CategoryId}, '{imageUrl}', {word.RepetitionNum});  SELECT SCOPE_IDENTITY();";

//        response.Data = Convert.ToInt32(await command.ExecuteScalarAsync());
//    }
//    catch (Exception ex)
//    {
//        response.Data = 0;
//        response.Message = ex.Message;
//        response.StatusCode = 500;
//    }
//    return response;
//}

//public async Task<string> GetImageUrlAsync(string wordName)
//{
//    var apiKey = _configuration["GoogleCustomSearch:ApiKey"];
//    var searchEngineId = _configuration["GoogleCustomSearch:SearchEngineId"];

//    var query = $"{wordName} image";
//    var url = $"https://www.googleapis.com/customsearch/v1?q={query}&cx={searchEngineId}&key={apiKey}&searchType=image";

//    try
//    {
//        var response = await _httpClient.GetAsync(url);
//        response.EnsureSuccessStatusCode();

//        var json = await response.Content.ReadAsStringAsync();
//        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

//        string imageUrl = data.items[0].link;
//        return imageUrl;
//    }
//    catch (Exception ex)
//    {
//        // Handle exception
//        Console.WriteLine($"Error fetching image URL: {ex.Message}");
//        return null;
//    }
//}