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

        public async Task<Billing?> GetByIdAsync(Guid id)
        {
            var bill = await _db.Billings.FirstOrDefaultAsync(b => b.Id == id);
            if (bill == null) return null;

            var user = await _authDb.Users.FirstOrDefaultAsync(u => u.Id == bill.UserId.ToString());
            bill.UserEmail = user?.Email ?? "Unknown";

            // find start & end of this bill's month
            var (start, end) = ParseMonth(bill.Month);

            // get issued orders for this user within that month
            bill.Orders = await (from o in _db.Orders
                                 join p in _db.PantryItems on o.PantryItemId equals p.Id
                                 where o.UserId == bill.UserId
                                    && o.Status == OrderStatus.Issued
                                    && o.IssuedDate.HasValue
                                    && o.IssuedDate.Value >= start
                                    && o.IssuedDate.Value < end
                                 select new Order
                                 {
                                     Id = o.Id,
                                     UserId = o.UserId,
                                     PantryItemId = o.PantryItemId,
                                     PantryItem = p,
                                     Quantity = o.Quantity,
                                     Status = o.Status,
                                     RequestDate = o.RequestDate,
                                     IssuedDate = o.IssuedDate
                                 }).ToListAsync();

            return bill;
        }

        //public async Task<Billing?> GetByIdAsync(Guid id)
        //{
        //    var bill = await _db.Billings.FirstOrDefaultAsync(b => b.Id == id);
        //    if (bill == null) return null;

        //    var user = await _authDb.Users.FirstOrDefaultAsync(u => u.Id == bill.UserId.ToString());
        //    bill.UserEmail = user?.Email ?? "Unknown";

        //    return bill;
        //}

        public async Task AddAsync(Billing billing)
        {
            _db.Billings.Add(billing);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var bill = await _db.Billings.FindAsync(id);
            if (bill == null) return;

            _db.Billings.Remove(bill);
            await _db.SaveChangesAsync();
        }
        public async Task<Billing> GenerateForUserMonthAsync(Guid userId, string monthLabel)
        {
            var key = NormalizeMonth(monthLabel);
            var (start, _) = ParseMonth(key); // we just need the year & month

            int targetYear = start.Year;
            int targetMonth = start.Month;

            // Calculate monthly total for this user
            var total = await (from o in _db.Orders
                               join p in _db.PantryItems on o.PantryItemId equals p.Id
                               let effectiveDate = (DateTime?)(o.IssuedDate ?? o.RequestDate)
                               where o.UserId == userId
                                  && o.Status == OrderStatus.Issued
                                  && effectiveDate.Value.Year == targetYear
                                  && effectiveDate.Value.Month == targetMonth
                               select p.Price * o.Quantity)
                  .SumAsync(x => (decimal?)x) ?? 0m;


            // Check if bill already exists for that month
            var bill = await _db.Billings
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == key);

            if (bill == null)
            {
                // create new bill for this month
                bill = new Billing
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Month = key,                // e.g. "Aug-2025"
                    TotalAmount = total,
                    GeneratedDate = DateTime.UtcNow
                };
                _db.Billings.Add(bill);
            }
            else
            {
                // update existing bill (same month + user)
                bill.TotalAmount = total;
                bill.GeneratedDate = DateTime.UtcNow;
                _db.Billings.Update(bill);
            }

            await _db.SaveChangesAsync();

            // Attach User Email for display
            var user = await _authDb.Users.FirstOrDefaultAsync(u => u.Id == bill.UserId.ToString());
            bill.UserEmail = user?.Email ?? "Unknown";

            return bill;
        }


        //public async Task<Billing> GenerateForUserMonthAsync(Guid userId, string monthLabel)
        //{
        //    var key = NormalizeMonth(monthLabel);
        //    var (start, end) = ParseMonth(key);

        //    // Calculate monthly total for this user
        //    var total = await (from o in _db.Orders
        //                       join p in _db.PantryItems on o.PantryItemId equals p.Id
        //                       where o.UserId == userId
        //                          && o.Status == OrderStatus.Issued
        //                          && ((o.IssuedDate ?? o.RequestDate) >= start)
        //                          && ((o.IssuedDate ?? o.RequestDate) < end)
        //                       select p.Price * o.Quantity)
        //          .SumAsync(x => (decimal?)x) ?? 0m;


        //    // Check if bill already exists for that month
        //    var bill = await _db.Billings
        //        .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == key);

        //    if (bill == null)
        //    {
        //        // create new bill for this month
        //        bill = new Billing
        //        {
        //            Id = Guid.NewGuid(),
        //            UserId = userId,
        //            Month = key,                // e.g. "Aug-2025"
        //            TotalAmount = total,
        //            GeneratedDate = DateTime.UtcNow
        //        };
        //        _db.Billings.Add(bill);
        //    }
        //    else
        //    {
        //        // update existing bill (same month + user)
        //        bill.TotalAmount = total;
        //        bill.GeneratedDate = DateTime.UtcNow;
        //        _db.Billings.Update(bill);
        //    }

        //    await _db.SaveChangesAsync();

        //    // Attach User Email for display
        //    var user = await _authDb.Users.FirstOrDefaultAsync(u => u.Id == bill.UserId.ToString());
        //    bill.UserEmail = user?.Email ?? "Unknown";

        //    return bill;
        //}




        private static readonly string[] MonthFormats = { "MMM-yyyy", "MMMM-yyyy", "yyyy-MM", "MM-yyyy" };
        private static string NormalizeMonth(string label)
        {
            if (!DateTime.TryParseExact(label.Trim(), MonthFormats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                dt = DateTime.Parse(label, CultureInfo.InvariantCulture);
            return dt.ToString("MMM-yyyy", CultureInfo.InvariantCulture);
        }


        private static (DateTime start, DateTime end) ParseMonth(string canonical)
        {
            var d = DateTime.ParseExact(canonical, "MMM-yyyy", CultureInfo.InvariantCulture);
            var start = new DateTime(d.Year, d.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return (start, start.AddMonths(1));
        }
    }
}