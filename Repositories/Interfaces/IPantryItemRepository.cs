using PantryManagementSystem.Models.Domain;

namespace PantryManagementSystem.Repositories.Interfaces
{
    public interface IPantryItemRepository
    {
        Task<IEnumerable<PantryItem>> GetAllAsync();
        Task<PantryItem?> GetByIdAsync(Guid id);
        Task AddAsync(PantryItem item);
        Task UpdateAsync(PantryItem item);
        Task DeleteAsync(Guid id);
    }
}
