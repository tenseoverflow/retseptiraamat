using backend.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Interfaces    
{
    public interface IFridgeService
    {
        Task<IEnumerable<FridgeItem>> GetAllAsync();
        Task<FridgeItem> GetByIdAsync(int id);
        Task<FridgeItem> CreateAsync(FridgeItem fridgeItem);
        Task UpdateAsync(FridgeItem fridgeItem);
        Task DeleteAsync(int id);
        Task<bool> IngredientExistsAsync(string ingredientName);
        Task<Dictionary<string, int>> GetAggregatedIngredientsAsync();
    }
}