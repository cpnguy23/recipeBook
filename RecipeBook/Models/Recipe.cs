namespace RecipeBook.Models
{
    public class Recipe
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Time { get; set; } = "";
        public string Category { get; set; } = "";
        public string CategorySlug { get; set; } = "";

        public string Tag { get; set; } = ""; // e.g. POPULAR, NEW
        public string? PhotoUrl { get; set; } // user-uploaded photo (if any)
    }
}
