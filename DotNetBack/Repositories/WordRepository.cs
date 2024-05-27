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


namespace DotNetBack.Repositories
{
    public class WordRepository : IWordRepository
    {
        private readonly IConfiguration _configuration;

        public WordRepository(IConfiguration configuration)
        {
            _configuration = configuration;
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
                    response.Message = "Such word does not exist";
                    return response;
                }

                await command.ExecuteNonQueryAsync();
            }
            catch(Exception ex)
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
                         $"category_id = { word.CategoryId}, " +
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

                var command = connection.CreateCommand();

                command.CommandText = "INSERT INTO Word (name, translation, category_id, img_link, repetition_num)" +
                    $"VALUES ('{word.Name}', '{word.Translation}', {word.CategoryId}, '{word.ImgLink}', {word.RepetitionNum});  SELECT SCOPE_IDENTITY();";

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
    }
}