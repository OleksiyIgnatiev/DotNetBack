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

        public int WordId { get; set; }

        public string Name { get; private set; }

        public string Translation { get; private set; }

        public int? CategoryId { get; private set; }

        public string ImgLink { get; private set; }

        public int RepetitionNum { get; private set; }

        public DateTime RepetitionDate { get; private set; }

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