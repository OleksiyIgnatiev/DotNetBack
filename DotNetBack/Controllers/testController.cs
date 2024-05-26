using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotNetBack.Models;
using System.Collections.Generic;

namespace DotNetBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Test>> GetTestQuestions()
        {
            var questions = new List<Test>
            {
                new Test
                {
                    QuestionText = "Яблуко",
                    Answers = new List<string> { "Pear", "Orange", "Apple", "Banana" },
                    CorrectAnswerIndex = 2
                },
                new Test
                {
                    QuestionText = "Собака",
                    Answers = new List<string> { "Cat", "Dog", "Bird", "Fish" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Будинок",
                    Answers = new List<string> { "House", "Car", "Tree", "Street" },
                    CorrectAnswerIndex = 0
                },
                new Test
                {
                    QuestionText = "Книга",
                    Answers = new List<string> { "Pen", "Book", "Table", "Chair" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Стіл",
                    Answers = new List<string> { "Window", "Table", "Door", "Wall" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Місто",
                    Answers = new List<string> { "Village", "City", "Country", "Town" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Школа",
                    Answers = new List<string> { "Hospital", "School", "Library", "Market" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Вчитель",
                    Answers = new List<string> { "Student", "Doctor", "Teacher", "Engineer" },
                    CorrectAnswerIndex = 2
                },
                new Test
                {
                    QuestionText = "Море",
                    Answers = new List<string> { "River", "Lake", "Sea", "Ocean" },
                    CorrectAnswerIndex = 2
                },
                new Test
                {
                    QuestionText = "Гора",
                    Answers = new List<string> { "Hill", "Mountain", "Valley", "Field" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Квітка",
                    Answers = new List<string> { "Tree", "Flower", "Grass", "Bush" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Вода",
                    Answers = new List<string> { "Water", "Milk", "Juice", "Tea" },
                    CorrectAnswerIndex = 0
                },
                new Test
                {
                    QuestionText = "Вікно",
                    Answers = new List<string> { "Door", "Window", "Roof", "Wall" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Кіт",
                    Answers = new List<string> { "Dog", "Bird", "Cat", "Fish" },
                    CorrectAnswerIndex = 2
                },
                new Test
                {
                    QuestionText = "Дерево",
                    Answers = new List<string> { "Tree", "Flower", "Grass", "Bush" },
                    CorrectAnswerIndex = 0
                },
                new Test
                {
                    QuestionText = "Час",
                    Answers = new List<string> { "Day", "Night", "Hour", "Time" },
                    CorrectAnswerIndex = 3
                },
                new Test
                {
                    QuestionText = "Світло",
                    Answers = new List<string> { "Light", "Dark", "Bright", "Dim" },
                    CorrectAnswerIndex = 0
                },
                new Test
                {
                    QuestionText = "Любов",
                    Answers = new List<string> { "Hate", "Love", "Fear", "Joy" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Мрія",
                    Answers = new List<string> { "Dream", "Nightmare", "Thought", "Wish" },
                    CorrectAnswerIndex = 0
                },
                new Test
                {
                    QuestionText = "Знання",
                    Answers = new List<string> { "Ignorance", "Knowledge", "Information", "Wisdom" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Гордість",
                    Answers = new List<string> { "Pride", "Shame", "Honor", "Arrogance" },
                    CorrectAnswerIndex = 0
                },
                new Test
                {
                    QuestionText = "Загадка",
                    Answers = new List<string> { "Answer", "Riddle", "Joke", "Puzzle" },
                    CorrectAnswerIndex = 1
                },
                new Test
                {
                    QuestionText = "Мудрість",
                    Answers = new List<string> { "Foolishness", "Knowledge", "Wisdom", "Insight" },
                    CorrectAnswerIndex = 2
                },
                new Test
                {
                    QuestionText = "Спадщина",
                    Answers = new List<string> { "Heritage", "Inheritance", "Legacy", "Tradition" },
                    CorrectAnswerIndex = 0
                },
                new Test
                {
                    QuestionText = "Наполегливість",
                    Answers = new List<string> { "Persistence", "Stubbornness", "Tenacity", "Determination" },
                    CorrectAnswerIndex = 0
                }
            };

            return Ok(questions);
        }
    }
}
