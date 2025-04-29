using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using backend.Data;
using backend.Model;
using Microsoft.EntityFrameworkCore;
using backend.Interfaces;
using System.Linq;

namespace backend.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext _context;

        public RecipeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> GetAllAsync()
        {
            return await _context.Recipes.ToListAsync();
        }

        public async Task<Recipe?> GetByIdAsync(int id)
        {
            return await _context.Recipes.FindAsync(id);
        }

        public async Task<Recipe> CreateAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task UpdateAsync(Recipe recipe)
        {
            // Get the existing recipe from the database
            var existingRecipe = await _context.Recipes.FindAsync(recipe.Id);
            
            if (existingRecipe == null)
            {
                throw new KeyNotFoundException($"Recipe with ID {recipe.Id} not found");
            }
            
            // Update the properties of the existing entity
            existingRecipe.Title = recipe.Title;
            existingRecipe.Description = recipe.Description;
            existingRecipe.Ingredients = recipe.Ingredients;
            existingRecipe.UpdatedAt = DateTime.UtcNow;
            
            // Save changes
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }
    }
}