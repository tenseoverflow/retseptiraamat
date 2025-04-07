using System.ComponentModel.DataAnnotations;

namespace backend.Model
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public Dictionary<string, double> Ingredients { get; set; } = new ();
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}