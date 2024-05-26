﻿using DotNetBack.Models;

namespace DotNetBack.Repositories
{
    public interface IWordRepository
    {
        Task DeleteWordAsync(int wordId);

        Task<List<Word>> GetWordsAsync(int category_id);
       
        Task<int> AddWordAsync(Word category);

        Task UpdateWordAsync(Word word);
    }
}
 