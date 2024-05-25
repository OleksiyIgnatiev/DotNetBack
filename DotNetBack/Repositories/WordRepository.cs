using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using DotNetBack.Models;


namespace DotNetBack.Repositories
{
    public class WordRepository
    {
        private readonly IConfiguration _configuration;

        public WordRepository(IConfiguration configuration)
        {
            _configuration = configuration;
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

            using (var connection = new SqlConnection(_configuration.GetConnectionString("ppDBCon")))
            {
                await connection.OpenAsync();

                var result = await connection.QuerySingleAsync<int>(sql, new
                {
                    Name = word.Name,
                    Translation = word.Translation,
                    CategoryId = word.CategoryId,
                    ImgLink = word.ImgLink,
                    RepetitionNum = word.RepetitionNum,
                    RepetitionDate = word.RepetitionDate
                });

                return result;
            }
        }


    }
}
