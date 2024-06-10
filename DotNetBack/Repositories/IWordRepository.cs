using DotNetBack.Models;

namespace DotNetBack.Repositories
{
    public interface IWordRepository
    {
        Task<Response> DeleteWordAsync(int wordId);

        Task<Response> GetWordsAsync(int category_id);
       
        Task<Response> AddWordAsync(Word category);

        Task<Response> UpdateWordAsync(Word word);
        Task<string> GetImageUrlAsync(string wordName);

        Task<Response> RealizeWordAsync(int wordId);
    }
}
 