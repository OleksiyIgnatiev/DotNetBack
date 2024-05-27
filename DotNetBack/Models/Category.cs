using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace DotNetBack.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; } // Assuming a User class exists

        public int WordCount { get; set; }
        public float PercentLearned { get; set; }
    }
}

