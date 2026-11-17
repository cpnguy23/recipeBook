using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Models
{
    /// <summary>
    /// A user-submitted recipe, persisted in the database.
    /// </summary>
    public class RecipePost
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Title { get; set; } = "";

        [Required]
        public string Instructions { get; set; } = "";

        // "breakfast", "lunch", "dinner", "dessert", "snack", "drinks"
        [Required]
        public string CategorySlug { get; set; } = "";

        public string? PhotoFileName { get; set; }   // file under wwwroot/uploads
        public string? PhotoContentType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
