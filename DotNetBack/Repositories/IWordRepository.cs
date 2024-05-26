using DotNetBack.Models;

namespace DotNetBack.Repositories
{
    public interface IWordRepository
    {
        Task<int> DeleteWordAsync(int wordId);

        Task<List<Word>> GetWordsAsync(int category_id);
       
        Task<int> AddWordAsync(Word category);

        Task<int> UpdateWordAsync(Word word);
    }
}
 