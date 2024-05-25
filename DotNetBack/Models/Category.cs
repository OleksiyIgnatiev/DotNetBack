using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace DotNetBack.Models
{
    public class Category
    {
        [JsonIgnore]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int UserId { get; set; }
        public int WordCount { get; set; }
        public float PercentLearned { get; set; }
    }
}

