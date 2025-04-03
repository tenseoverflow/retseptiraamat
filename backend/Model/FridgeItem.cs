using System.ComponentModel.DataAnnotations;

namespace backend.Model
{
    public class FridgeItem
    {
        [Key]
        public int Id { get; set; }
        public Dictionary<string, double> Ingredients { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}