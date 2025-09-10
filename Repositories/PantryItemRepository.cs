using Microsoft.EntityFrameworkCore;
using PantryManagementSystem.Data;
using PantryManagementSystem.Models.Domain;
using PantryManagementSystem.Repositories.Interfaces;

namespace PantryManagementSystem.Repositories
{
    public class PantryItemRepository : IPantryItemRepository
    {
        private readonly PantryDbContext _context;

        public PantryItemRepository(PantryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PantryItem>> GetAllAsync()
        {
            return await _context.PantryItems.ToListAsync();
        }

        public async Task<PantryItem?> GetByIdAsync(Guid id)
        {
            return await _context.PantryItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(PantryItem item)
        {
            await _context.PantryItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PantryItem item)
        {
            _context.PantryItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _context.PantryItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                _context.PantryItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}