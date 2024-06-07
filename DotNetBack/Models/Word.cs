using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetBack.Models
{
    public class Word
    {
        public Word(
            int wordId,
            string name,
            string translation,
            int categoryId,
            string imgLink,
            int repetitionNum)
        {
            WordId = wordId;
            Name = name;
            Translation = translation;
            CategoryId = categoryId;
            ImgLink = imgLink;
            RepetitionNum = repetitionNum;
        }

        public Word() { }

        public int WordId { get; set; }


        [Required]
        public string Name { get;  set; }

        [Required]
        public string Translation { get;  set; }

        [Required]
        public int CategoryId { get;  set; }

        public string? ImgLink { get; set; }

        [Required]
        public int RepetitionNum { get;  set; }


        public static Word Create(
            int wordId,
            string name,
            string translation,
            int categoryId,
            string imgLink,
            int repetitionNum)
        {
            return new Word(wordId, name, translation, categoryId, imgLink, repetitionNum);
        }
    }
    
}