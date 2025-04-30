using System.Threading.Tasks;
using System.Collections.Generic;
using backend.Data;
using backend.Model;
using Microsoft.EntityFrameworkCore;
using backend.Interfaces;

namespace backend.Services
{
    public class FridgeService : IFridgeService
    {
        private readonly ApplicationDbContext _context;

        public FridgeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FridgeItem>> GetAllAsync()
        {
            return await _context.FridgeItems.ToListAsync();
        }

        public async Task<FridgeItem> GetByIdAsync(int id)
        {
            var fridgeItem = await _context.FridgeItems.FindAsync(id);
            if (fridgeItem == null)
            {
                throw new KeyNotFoundException($"FridgeItem with id {id} not found");
            }
            return fridgeItem;
        }

        public async Task<FridgeItem> CreateAsync(FridgeItem fridgeItem)
        {
            fridgeItem.LastUpdated = DateTime.UtcNow;
            _context.FridgeItems.Add(fridgeItem);
            await _context.SaveChangesAsync();
            return fridgeItem;
        }

        public async Task UpdateAsync(FridgeItem fridgeItem)
        {
            // Fix the tracking issue by fetching the entity first and then updating its properties
            var existingItem = await _context.FridgeItems.FindAsync(fridgeItem.Id);
            if (existingItem != null)
            {
                // Update the properties of the tracked entity
                existingItem.Ingredient = fridgeItem.Ingredient;
                existingItem.Amount = fridgeItem.Amount;
                existingItem.LastUpdated = DateTime.UtcNow;
                
                // Save changes
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var fridgeItem = await _context.FridgeItems.FindAsync(id);
            if (fridgeItem != null)
            {
                _context.FridgeItems.Remove(fridgeItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IngredientExistsAsync(string ingredientName)
        {
            // Check if an ingredient with the same name already exists (case-insensitive comparison)
            return await _context.FridgeItems.AnyAsync(i => 
                i.Ingredient.ToLower() == ingredientName.ToLower());
        }

        public async Task<Dictionary<string, int>> GetAggregatedIngredientsAsync()
        {
            var allIngredients = await _context.FridgeItems.ToListAsync();
            var aggregatedIngredients = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            foreach (var item in allIngredients)
            {
                if (aggregatedIngredients.ContainsKey(item.Ingredient))
                {
                    aggregatedIngredients[item.Ingredient] += item.Amount;
                }
                else
                {
                    aggregatedIngredients.Add(item.Ingredient, item.Amount);
                }
            }
            
            return aggregatedIngredients;
        }
    }
}