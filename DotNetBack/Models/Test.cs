namespace DotNetBack.Models
{
    public class Test
    {
        public string QuestionText { get; set; } // Текст вопроса
        public List<string> Answers { get; set; } // Список вариантов ответа
        public int CorrectAnswerIndex { get; set; } // Индекс правильного ответа в списке вариантов
    }

}
