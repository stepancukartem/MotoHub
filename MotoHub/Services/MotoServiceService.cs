
using Microsoft.EntityFrameworkCore;
using MotoHub.DAL;
using MotoHub.DAL.Entities;
using MotoHub.DTOs;

namespace MotoHub.Services
{
    public class MotoServiceService
    {
        private readonly AppDbContext _context;

        public MotoServiceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceDto>> GetAllAsync()
        {
            return await _context.Services
                .AsNoTracking()
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    DurationMinutes = s.DurationMinutes
                })
                .ToListAsync();
        }

        public async Task<ServiceDto?> GetByIdAsync(int id)
        {
            return await _context.Services
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    DurationMinutes = s.DurationMinutes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceDto?> CreateAsync(
            CreateServiceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Description) ||
                dto.Name.Trim().Length > 200 ||
                dto.Description.Trim().Length > 1000 ||
                dto.Price < 0 ||
                dto.DurationMinutes <= 0)
            {
                return null;
            }

            var service = new Service
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                Price = dto.Price,
                DurationMinutes = dto.DurationMinutes
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(service.Id);
        }

        public async Task<bool> UpdateAsync(
            int id,
            CreateServiceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Description) ||
                dto.Name.Trim().Length > 200 ||
                dto.Description.Trim().Length > 1000 ||
                dto.Price < 0 ||
                dto.DurationMinutes <= 0)
            {
                return false;
            }

            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return false;

            service.Name = dto.Name.Trim();
            service.Description = dto.Description.Trim();
            service.Price = dto.Price;
            service.DurationMinutes = dto.DurationMinutes;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
