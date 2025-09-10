using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PantryManagementSystem.Data;
using PantryManagementSystem.Models.Domain;
using PantryManagementSystem.Models.DTO;
using PantryManagementSystem.Models.Enums;
using PantryManagementSystem.Repositories.Interfaces;

namespace PantryManagementSystem.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PantryDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderRepository(PantryDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<PantryItem>> BrowseAsync()
        {
            return await _context.PantryItems
                .Where(p => p.Quantity > 0 && p.ExpiryDate >= DateTime.Now)
                .ToListAsync();
        }

        public async Task<Order> RequestItemAsync(Guid userId, Guid pantryItemId, int quantity)
        {
            var item = await _context.PantryItems.FindAsync(pantryItemId);
            if (item == null || item.Quantity < quantity) return null;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PantryItemId = pantryItemId,
                Quantity = quantity,
                Status = OrderStatus.Pending,
                RequestDate = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<IEnumerable<Order>> GetMyOrdersAsync(Guid userId)
        {
            return await _context.Orders
                .Include(o => o.PantryItem)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.RequestDate)
                .ToListAsync();
        }

        // ✅ Get All Orders with User Email
        public async Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.PantryItem)
                .OrderByDescending(o => o.RequestDate)
                .ToListAsync();

            var result = new List<OrderReadDTO>();

            foreach (var o in orders)
            {
                // Convert Guid to string for Identity lookup
                var user = await _userManager.FindByIdAsync(o.UserId.ToString());

                result.Add(new OrderReadDTO
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    //UserEmail = user?.UserName ?? "Unknown", // ✅ get Email
                    PantryItemName = o.PantryItem.Name,
                    Quantity = o.Quantity,
                    Status = o.Status.ToString(),
                    RequestDate = o.RequestDate,
                    IssuedDate = o.IssuedDate
                });
            }

            return result;
        }

        public async Task<IEnumerable<MyOrderDTO>> GetAllOrdersForStaffAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.PantryItem)
                .OrderByDescending(o => o.RequestDate)
                .ToListAsync();

            var result = new List<MyOrderDTO>();

            foreach (var o in orders)
            {
                result.Add(new MyOrderDTO
                {
                    Id = o.Id,
                    PantryItemName = o.PantryItem.Name,
                    Quantity = o.Quantity,
                    Status = o.Status.ToString(),
                    RequestDate = o.RequestDate,
                    IssuedDate = o.IssuedDate
                });
            }

            return result;
        }


        public async Task<Order> ApproveOrderAsync(Guid orderId)
        {
            var order = await _context.Orders.Include(o => o.PantryItem).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return null;

            order.Status = OrderStatus.Approved;
            order.IssuedDate = DateTime.UtcNow;
            order.PantryItem.Quantity -= order.Quantity;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> DenyOrderAsync(Guid orderId)
        {
            var order = await _context.Orders.Include(o => o.PantryItem).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return null;

            order.Status = OrderStatus.Denied;
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> IssueOrderAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.PantryItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.Status != OrderStatus.Approved) return null;
            if (order.PantryItem.Quantity < order.Quantity) return null;

            order.PantryItem.Quantity -= order.Quantity;
            order.Status = OrderStatus.Issued;
            order.IssuedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> GetByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.PantryItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
