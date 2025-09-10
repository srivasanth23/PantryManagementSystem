using Microsoft.EntityFrameworkCore;
using PantryManagementSystem.Data;
using PantryManagementSystem.Models.Domain;
using PantryManagementSystem.Models.Enums;
using PantryManagementSystem.Repositories.Interfaces;
using System.Globalization;

namespace PantryManagementSystem.Repositories
{
    public class BillingRepository : IBillingRepository
    {
        private readonly PantryDbContext _db;
        private readonly AuthDbContext _authDb;

        public BillingRepository(PantryDbContext db, AuthDbContext authDb)
        {
            _db = db;
            _authDb = authDb;
        }

        // ✅ Get all bills with UserEmail stitched in memory
        public async Task<IEnumerable<Billing>> GetAllAsync()
        {
            var bills = await _db.Billings
                .OrderByDescending(b => b.GeneratedDate)
                .ToListAsync();

            var users = await _authDb.Users.ToListAsync();

            foreach (var bill in bills)
            {
                var user = users.FirstOrDefault(u => u.Id == bill.UserId.ToString());
                bill.UserEmail = user?.Email ?? "Unknown";
            }

            return bills;
        }

        // ✅ Get bills for a single user
        public async Task<IEnumerable<Billing>> GetByUserAsync(Guid userId)
        {
            var bills = await _db.Billings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.GeneratedDate)
                .ToListAsync();

            var user = await _authDb.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString());

            foreach (var bill in bills)
            {
                bill.UserEmail = user?.Email ?? "Unknown";
            }

            return bills;
        }

        // ✅ Get a single bill by Id
        public async Task<Billing?> GetByIdAsync(Guid id)
        {
            var bill = await _db.Billings.FirstOrDefaultAsync(b => b.Id == id);
            if (bill == null) return null;

            var user = await _authDb.Users.FirstOrDefaultAsync(u => u.Id == bill.UserId.ToString());
            bill.UserEmail = user?.Email ?? "Unknown";

            return bill;
        }

        public async Task AddAsync(Billing billing)
        {
            _db.Billings.Add(billing);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var x = await _db.Billings.FindAsync(id);
            if (x == null) return;
            _db.Billings.Remove(x);
            await _db.SaveChangesAsync();
        }

        // ✅ Generate bill logic stays same
        public async Task<Billing> GenerateForUserMonthAsync(Guid userId, string monthLabel)
        {
            var key = NormalizeMonth(monthLabel);
            var (start, end) = ParseMonth(key);

            var total = await (from o in _db.Orders
                               join p in _db.PantryItems on o.PantryItemId equals p.Id
                               where o.UserId == userId
                                  && o.Status == OrderStatus.Issued
                                  && o.IssuedDate.HasValue
                                  && o.IssuedDate.Value >= start
                                  && o.IssuedDate.Value < end
                               select p.Price * o.Quantity)
                              .SumAsync(x => (decimal?)x) ?? 0m;

            var bill = await _db.Billings
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == key);

            if (bill is null)
            {
                bill = new Billing
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Month = key,
                    TotalAmount = total,
                    GeneratedDate = DateTime.UtcNow
                };
                _db.Billings.Add(bill);
            }
            else
            {
                bill.TotalAmount = total;
                bill.GeneratedDate = DateTime.UtcNow;
                _db.Billings.Update(bill);
            }

            await _db.SaveChangesAsync();

            // Attach email
            var user = await _authDb.Users.FirstOrDefaultAsync(u => u.Id == bill.UserId.ToString());
            bill.UserEmail = user?.Email ?? "Unknown";

            return bill;
        }

        private static string NormalizeMonth(string label)
        {
            label = (label ?? "").Trim();

            if (label.StartsWith("Sept-", StringComparison.OrdinalIgnoreCase))
                label = "Sep-" + label.Substring(5);

            var formats = new[] { "MMM-yyyy", "MMMM-yyyy" };
            if (!DateTime.TryParseExact(label, formats, CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out var d))
                d = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            return d.ToString("MMM-yyyy", CultureInfo.InvariantCulture);
        }

        private static (DateTime start, DateTime end) ParseMonth(string canonical)
        {
            var d = DateTime.ParseExact(canonical, "MMM-yyyy", CultureInfo.InvariantCulture);
            var start = new DateTime(d.Year, d.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return (start, start.AddMonths(1));
        }
    }
}
