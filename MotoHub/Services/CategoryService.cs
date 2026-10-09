
using Microsoft.EntityFrameworkCore;
using MotoHub.DAL;
using MotoHub.DAL.Entities;
using MotoHub.DTOs;

namespace MotoHub.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CategoryDto?> CreateAsync(
            CreateCategoryDto dto)
        {
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return null;

            var exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());

            if (exists)
                return null;

            var category = new Category { Name = name };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> UpdateAsync(
            int id,
            CreateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return false;

            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return false;

            var exists = await _context.Categories.AnyAsync(
                c => c.Id != id &&
                     c.Name.ToLower() == name.ToLower());

            if (exists)
                return false;

            category.Name = name;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Motorcycles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null || category.Motorcycles.Count > 0)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
