using DotNetBack.Models;

namespace DotNetBack.Repositories
{
    public interface IWordRepository
    {
        Task<Response> DeleteWordAsync(int wordId);

        Task<List<Word>> GetWordsAsync(int category_id);
       
        Task<Response> AddWordAsync(Word category);

        Task<Response> UpdateWordAsync(Word word);
    }
}
 