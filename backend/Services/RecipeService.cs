using backend.Data;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext _context;

        public RecipeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _context.Recipes.ToList();
        }

        public Recipe GetRecipe(int id)
        {
            return _context.Recipes.Find(id);
        }

        public void AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            _context.SaveChanges();
        }
    }
}