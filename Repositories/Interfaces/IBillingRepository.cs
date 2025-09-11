using PantryManagementSystem.Models.Domain;

namespace PantryManagementSystem.Repositories.Interfaces
{
    public interface IBillingRepository
    {
        Task<IEnumerable<Billing>> GetAllAsync();//returns every Billling row
        Task<IEnumerable<Billing>> GetByUserAsync(Guid userId);//returns all bills that belong to one user
        Task<Billing?> GetByIdAsync(Guid id);
        Task<Billing> GenerateForUserMonthAsync(Guid userId, String monthLabel);//create if needed + return a monthly bill
        Task AddAsync(Billing billing); //creates a new record billing

        Task DeleteAsync(Guid id); // remove records by PK


    }
}