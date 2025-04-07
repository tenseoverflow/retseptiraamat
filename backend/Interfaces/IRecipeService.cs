using backend.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetAllAsync();
        Task<Recipe> GetByIdAsync(int id);
        Task<Recipe> CreateAsync(Recipe recipe);
        Task UpdateAsync(Recipe recipe);
        Task DeleteAsync(int id);
    }
}