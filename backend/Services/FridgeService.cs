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
            fridgeItem.LastUpdated = DateTime.UtcNow;
            _context.Entry(fridgeItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
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
    }
}