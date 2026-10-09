
using Microsoft.EntityFrameworkCore;
using MotoHub.DAL;
using MotoHub.DAL.Entities;
using MotoHub.DTOs;

namespace MotoHub.Services
{
    public class BrandService
    {
        private readonly AppDbContext _context;

        public BrandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BrandDto>> GetAllAsync()
        {
            return await _context.Brands
                .AsNoTracking()
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();
        }

        public async Task<BrandDto?> GetByIdAsync(int id)
        {
            return await _context.Brands
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BrandDto?> CreateAsync(CreateBrandDto dto)
        {
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return null;

            var exists = await _context.Brands
                .AnyAsync(b => b.Name.ToLower() == name.ToLower());

            if (exists)
                return null;

            var brand = new Brand { Name = name };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateBrandDto dto)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
                return false;

            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return false;

            var exists = await _context.Brands.AnyAsync(
                b => b.Id != id &&
                     b.Name.ToLower() == name.ToLower());

            if (exists)
                return false;

            brand.Name = name;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands
                .Include(b => b.Motorcycles)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null || brand.Motorcycles.Count > 0)
                return false;

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
