using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetBack.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int UserId { get; set; }
    }
}
