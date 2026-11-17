using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecipeBook.Models
{
    /// <summary>
    /// View model for the "Add New &gt; Recipe" upload form.
    /// </summary>
    public class RecipeUploadViewModel
    {
        [Required, StringLength(120)]
        public string Title { get; set; } = "";

        [Required]
        public string Instructions { get; set; } = "";

        [Required]
        public string CategorySlug { get; set; } = "";

        public IFormFile? Photo { get; set; }

        public SelectList CategoryOptions { get; set; } = new(
            new[] { "breakfast", "lunch", "dinner", "dessert", "snack", "drinks" });

        public string? ResultMessage { get; set; }
    }

    /// <summary>
    /// View model for creating/editing a meal type (category).
    /// </summary>
    public class MealTypeViewModel
    {
        [Required, StringLength(60)]
        public string Name { get; set; } = "";

        public string? ResultMessage { get; set; }
    }
}
