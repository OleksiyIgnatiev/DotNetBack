using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetBack.Models
{
    public class Word
    {
        private Word(
            int wordId,
            string name,
            string translation,
            int? categoryId,
            string imgLink,
            int repetitionNum,
            DateTime repetitionDate)
        {
            WordId = wordId;
            Name = name;
            Translation = translation;
            CategoryId = categoryId;
            ImgLink = imgLink;
            RepetitionNum = repetitionNum;
            RepetitionDate = repetitionDate;
        }

        public Word() { }

        [JsonIgnore]
        public int WordId { get; set; }


        [Required]
        public string Name { get;  set; }

        [Required]
        public string Translation { get;  set; }

        [Required]
        public int? CategoryId { get;  set; }

        [Required]
        public string ImgLink { get;  set; }

        [Required]
        public int RepetitionNum { get;  set; }

        [Required]
        public DateTime RepetitionDate { get;  set; }

        public static Word Create(
            int wordId,
            string name,
            string translation,
            int? categoryId,
            string imgLink,
            int repetitionNum,
            DateTime repetitionDate)
        {
            return new Word(wordId, name, translation, categoryId, imgLink, repetitionNum, repetitionDate);
        }
    }
}