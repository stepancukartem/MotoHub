
using Microsoft.EntityFrameworkCore;
using MotoHub.DAL;
using MotoHub.DAL.Entities;
using MotoHub.DTOs;

namespace MotoHub.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto?> CreateAsync(
            int userId,
            CreateOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return null;

            if (dto.Items.Any(i => i.Quantity <= 0))
                return null;

            if (dto.Items
                .GroupBy(i => i.MotorcycleId)
                .Any(g => g.Count() > 1))
            {
                return null;
            }

            var motorcycleIds = dto.Items
                .Select(i => i.MotorcycleId)
                .ToList();

            var motorcycles = await _context.Motorcycles
                .Where(m => motorcycleIds.Contains(m.Id))
                .ToListAsync();

            if (motorcycles.Count != motorcycleIds.Count)
                return null;

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = "New",
                TotalPrice = 0
            };

            foreach (var item in dto.Items)
            {
                var motorcycle = motorcycles
                    .First(m => m.Id == item.MotorcycleId);

                order.Items.Add(new OrderItem
                {
                    MotorcycleId = motorcycle.Id,
                    Quantity = item.Quantity,
                    Price = motorcycle.Price
                });

                order.TotalPrice += motorcycle.Price * item.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(order.Id, userId, false);
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Motorcycle)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapOrder).ToList();
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                    .ThenInclude(i => i.Motorcycle)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapOrder).ToList();
        }

        public async Task<OrderDto?> GetByIdAsync(
            int orderId,
            int userId,
            bool isAdmin)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                    .ThenInclude(i => i.Motorcycle)
                .Where(o => o.Id == orderId);

            if (!isAdmin)
                query = query.Where(o => o.UserId == userId);

            var order = await query.FirstOrDefaultAsync();

            return order == null ? null : MapOrder(order);
        }

        public async Task<bool> UpdateStatusAsync(
            int orderId,
            string status)
        {
            var allowedStatuses = new[]
            {
                "New",
                "Processing",
                "Shipped",
                "Completed",
                "Cancelled"
            };

            if (!allowedStatuses.Contains(status))
                return false;

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            order.Status = status;
            await _context.SaveChangesAsync();

            return true;
        }

        private static OrderDto MapOrder(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                UserId = order.UserId,

                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    MotorcycleId = i.MotorcycleId,
                    MotorcycleName = i.Motorcycle.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
        }
    }
}
