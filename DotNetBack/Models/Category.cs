namespace DotNetBack.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? UserId { get; set; }

        public Category(int categoryId, string categoryName, int? userId)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            UserId = userId;
        }

        public Category() { }

        public static Category Create(int categoryId, string categoryName, int? userId)
        {
            return new Category(categoryId, categoryName, userId);
        }
    }
}
