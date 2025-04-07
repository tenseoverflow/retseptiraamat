using System.ComponentModel.DataAnnotations;

namespace backend.Model
{
    public class FridgeItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Dictionary<string, double> Ingredients { get; set; } = new ();
        
        public DateTime LastUpdated { get; set; }
    }
}