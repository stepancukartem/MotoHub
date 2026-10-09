using MotoHub.DAL;
using Microsoft.EntityFrameworkCore;
using MotoHub.DAL.Entities;
using MotoHub.DAL.Repositories.Interfaces;

namespace MotoHub.DAL.Repositories
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        private readonly AppDbContext _context;

        public MotorcycleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Motorcycle>> GetAllAsync()
        {
            return await _context.Motorcycles
                .Include(m => m.Brand)
                .Include(m => m.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Motorcycle?> GetByIdAsync(int id)
        {
            return await _context.Motorcycles
                .Include(m => m.Brand)
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public Task<bool> BrandExistsAsync(int brandId)
        {
            return _context.Brands.AnyAsync(b => b.Id == brandId);
        }

        public Task<bool> CategoryExistsAsync(int categoryId)
        {
            return _context.Categories.AnyAsync(c => c.Id == categoryId);
        }

        public async Task AddAsync(Motorcycle motorcycle)
        {
            await _context.Motorcycles.AddAsync(motorcycle);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Delete(Motorcycle motorcycle)
        {
            _context.Motorcycles.Remove(motorcycle);
        }
    }
}
