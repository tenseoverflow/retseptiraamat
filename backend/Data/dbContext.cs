using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using backend.Model;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        private static readonly JsonSerializerOptions _jsonOptions = new();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<FridgeItem> FridgeItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>()
                .Property(e => e.Ingredients)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, typeof(Dictionary<string, double>), _jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, double>>(v, _jsonOptions) ?? new Dictionary<string, double>()
                );

            modelBuilder.Entity<FridgeItem>()
                .Property(e => e.Ingredient)
                .HasConversion(
                    v => v,
                    v => v ?? string.Empty
                );
        }
    }
}