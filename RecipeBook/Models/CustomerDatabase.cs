using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Models
{
    public class CustomerDatabase
    {
        [Required(ErrorMessage ="Name is required")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email. Please try again.")]
        public string Email { get; set; }

        public string Password { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Message cannot exceed 255 characters")]
        public string Message { get; set; }

    }
}
