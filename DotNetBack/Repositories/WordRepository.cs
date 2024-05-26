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




            return words;
        }

        public async Task<int> DeleteWordAsync(int wordId)
        {
            string sql = "DELETE FROM Word WHERE id = @Id";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
            {
                await connection.OpenAsync();

                var result = await connection.ExecuteAsync(sql, new { Id = wordId });

                return result;
            }
        }

        public async Task<int> UpdateWordAsync(Word word)
        {
            string sql = "UPDATE Word SET " +
                         "name = @Name, " +
                         "translation = @Translation, " +
                         "category_id = @CategoryId, " +
                         "img_link = @ImgLink, " +
                         "repetition_num = @RepetitionNum, " +
                         "repetition_date = @RepetitionDate " +
                         "WHERE id = @Id";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
            {
                await connection.OpenAsync();

                var result = await connection.ExecuteAsync(sql, new
                {
                    Name = word.Name,
                    Translation = word.Translation,
                    CategoryId = word.CategoryId,
                    ImgLink = word.ImgLink,
                    RepetitionNum = word.RepetitionNum,
                    RepetitionDate = word.RepetitionDate,
                    Id = word.WordId
                });

                return result;
            }
        }


        public async Task<int> AddWordAsync(Word word)
        {
            string sql = "INSERT INTO Word (name, translation, category_id, img_link, repetition_num, repetition_date) " +
                         "OUTPUT INSERTED.ID " + // если нужно вернуть ID вставленной строки
                         "VALUES (@Name, @Translation, @CategoryId, @ImgLink, @RepetitionNum, @RepetitionDate)";

            var connection = GetConnection();

            try
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();

                command.CommandText = "\"INSERT INTO Word (name, translation, category_id, img_link, repetition_num, repetition_date)" +
                    $"VALUES ({word.Name}, {word.Translation}, {word.CategoryId}, {word.ImgLink}, {word.RepetitionNum}, {word.RepetitionDate});" +
                    "SELECT SCOPE_IDENTITY();";

                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}