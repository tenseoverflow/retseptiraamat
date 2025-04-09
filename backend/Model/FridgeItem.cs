using System.ComponentModel.DataAnnotations;

namespace backend.Model
{
    public class FridgeItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Ingredient { get; set; }
        [Required]
        public int Amount { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }
    }
}