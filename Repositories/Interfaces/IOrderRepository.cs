using PantryManagementSystem.Models.Domain;
using PantryManagementSystem.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PantryManagementSystem.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        // Browse available pantry items
        Task<IEnumerable<PantryItem>> BrowseAsync();

        // Request an item - PLace an order 
        Task<Order> RequestItemAsync(Guid userId, Guid pantryItemId, int quantity);

        // Get orders of a specific user
        Task<IEnumerable<Order>> GetMyOrdersAsync(Guid userId);

        // Get all approved orders for a specific user
        Task<IEnumerable<Order>> GetApprovedOrdersAsync(Guid userId);

        // Get all orders with user email
        Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync();

        // Get all orders for staff view
        Task<IEnumerable<MyOrderDTO>> GetAllOrdersForStaffAsync();



        // Approve an order
        Task<Order> ApproveOrderAsync(Guid orderId);

        // Deny an order
        Task<Order> DenyOrderAsync(Guid orderId);

        // Issue an order
        Task<Order> IssueOrderAsync(Guid orderId);

        // Get order by Id
        Task<Order> GetByIdAsync(Guid orderId);
    }
}