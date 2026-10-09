
using AutoMapper;
using MotoHub.DAL.Entities;
using MotoHub.DAL.Repositories.Interfaces;
using MotoHub.DTOs;

namespace MotoHub.Services
{
    public class MotorcycleService
    {
        private readonly IMotorcycleRepository _repository;
        private readonly IMapper _mapper;

        public MotorcycleService(
            IMotorcycleRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<MotorcycleDto>> GetAllAsync()
        {
            var motorcycles = await _repository.GetAllAsync();

            return _mapper.Map<List<MotorcycleDto>>(motorcycles);
        }

        public async Task<MotorcycleDto?> GetByIdAsync(int id)
        {
            var motorcycle = await _repository.GetByIdAsync(id);

            return motorcycle == null
                ? null
                : _mapper.Map<MotorcycleDto>(motorcycle);
        }

        public async Task<MotorcycleDto?> CreateAsync(
            CreateMotorcycleDto dto)
        {
            if (!await _repository.BrandExistsAsync(dto.BrandId) ||
                !await _repository.CategoryExistsAsync(dto.CategoryId))
            {
                return null;
            }

            var motorcycle = _mapper.Map<Motorcycle>(dto);

            await _repository.AddAsync(motorcycle);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(motorcycle.Id);

            return created == null
                ? null
                : _mapper.Map<MotorcycleDto>(created);
        }

        public async Task<MotorcycleDto?> UpdateAsync(
            int id,
            UpdateMotorcycleDto dto)
        {
            var motorcycle = await _repository.GetByIdAsync(id);

            if (motorcycle == null)
                return null;

            if (!await _repository.BrandExistsAsync(dto.BrandId) ||
                !await _repository.CategoryExistsAsync(dto.CategoryId))
            {
                return null;
            }

            _mapper.Map(dto, motorcycle);

            await _repository.SaveChangesAsync();

            var updated = await _repository.GetByIdAsync(id);

            return updated == null
                ? null
                : _mapper.Map<MotorcycleDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var motorcycle = await _repository.GetByIdAsync(id);

            if (motorcycle == null)
                return false;

            _repository.Delete(motorcycle);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
