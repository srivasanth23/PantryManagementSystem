using Microsoft.EntityFrameworkCore;
using PantryManagementSystem.Models.Domain;
using PantryManagementSystem.Models.Enums;

namespace PantryManagementSystem.Data
{
    public class PantryDbContext : DbContext
    {
        public PantryDbContext(DbContextOptions<PantryDbContext> options) : base(options)
        {
        }

        public DbSet<PantryItem> PantryItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Billing> Billings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Store Enum as string in DB (easier to read than int values)
            modelBuilder
                .Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(OrderStatus.Pending);

            // ✅ Seed Pantry Items with static values
            var items = new List<PantryItem>
            {
                new PantryItem
                {
                    Id = Guid.Parse("aec40d63-08c4-42b5-92e6-f5cc46187222"),
                    Name = "Tea Powder",
                    Category = "Beverage",
                    Price = 10,
                    Quantity = 100,
                    ExpiryDate = new DateTime(2025, 12, 31)
                },
                new PantryItem
                {
                    Id = Guid.Parse("91555176-1298-4383-8e3e-bb0695861ee7"),
                    Name = "Coffee Powder",
                    Category = "Beverage",
                    Price = 20,
                    Quantity = 50,
                    ExpiryDate = new DateTime(2025, 12, 31)
                }
            };
            modelBuilder.Entity<PantryItem>().HasData(items);

            var order = new Order
            {
                Id = Guid.Parse("aca1f837-fba7-42d2-ae01-7f25ccfc1653"),
                UserId = Guid.Parse("20e32654-1f4f-4e36-8c1d-7a805d5218dd"), 
                PantryItemId = Guid.Parse("91555176-1298-4383-8e3e-bb0695861ee7"),
                Quantity = 2,
                Status = OrderStatus.Pending,
                RequestDate = new DateTime(2025, 09, 01) 
            };
            modelBuilder.Entity<Order>().HasData(order);

            var billing = new Billing
            {
                Id = Guid.Parse("c8f2e7c2-3af8-4a8a-91d1-28e5f5ffdb92"),
                UserId = Guid.Parse("20e32654-1f4f-4e36-8c1d-7a805d5218dd"), 
                Month = "Sep-2025",
                TotalAmount = 40, // Example: user consumed ₹40 worth items
                GeneratedDate = new DateTime(2025, 09, 05) 
            };
            modelBuilder.Entity<Billing>().HasData(billing);
        }
    }
}
