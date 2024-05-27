using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using DotNetBack.Models;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


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

        public async Task<List<Word>> GetWordsAsync(int category_id)
        {
            List<Word> words = new List<Word>();

            SqlConnection connection = GetConnection();

            try
            {
                await connection.OpenAsync();

            var command = connection.CreateCommand();


            command.CommandText = "SELECT * FROM Word WHERE category_id = @category_id";
            command.Parameters.AddWithValue("@category_id", category_id);

            var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                int word_id = Convert.ToInt32(reader["word_id"]);
                string name = reader["name"].ToString();
                string translation = reader["translation"].ToString();
                string img_link = reader["translation"].ToString();
                int repetition_num = Convert.ToInt32(reader["repetition_num"]);
                DateTime repetition_date = Convert.ToDateTime(reader["repetition_date"]);

                Word word = Word.Create(word_id, name, translation, category_id, img_link, repetition_num, repetition_date);

                words.Add(word);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }




            return words;
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

                await command.ExecuteNonQueryAsync();
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
            }
            return response;

        }

       /* public async Task<Response> UpdateWordAsync(Word word)
        {
            Response response = new Response();
            string date = $"{word.RepetitionDate.Year}-{word.RepetitionDate.Month}-{word.RepetitionDate.Day}";
            string sql = "UPDATE Word SET " +
                         $"name = '{word.Name}', " +
                         $"translation = '{word.Translation}', " +
                         $"category_id = { word.CategoryId}, " +
                         $"img_link = '{word.ImgLink}', " +
                         $"repetition_num = {word.RepetitionNum}, " +
                         $"repetition_date = '{date}' " +
                         $"WHERE word_id = {word.WordId}";

            var connection = GetConnection();
            try
            {
                var command = connection.CreateCommand();
                command.CommandText = sql;

                await connection.OpenAsync();

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
            }
            return response;
        }*/

        /*public async Task<int> AddWordAsync(Word word)
        {
            Response response = new Response();
            var connection = GetConnection();

            try
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();

                string date = $"{word.RepetitionDate.Year}-{word.RepetitionDate.Month}-{word.RepetitionDate.Day}";
                command.CommandText = "INSERT INTO Word (name, translation, category_id, img_link, repetition_num, repetition_date)" +
                    $"VALUES ('{word.Name}', '{word.Translation}', {word.CategoryId}, '{word.ImgLink}', {word.RepetitionNum}, '{date}');  SELECT SCOPE_IDENTITY();";

                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                return 0;
            }
        }*/
    }
}