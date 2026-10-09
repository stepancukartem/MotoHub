using MotoHub.DAL.Entities;
using MotoHub.DAL;
namespace MotoHub.DAL.Repositories.Interfaces
{
    public interface IMotorcycleRepository
    {
        Task<List<Motorcycle>> GetAllAsync();
        Task<Motorcycle?> GetByIdAsync(int id);
        Task<bool> BrandExistsAsync(int brandId);
        Task<bool> CategoryExistsAsync(int categoryId);
        Task AddAsync(Motorcycle motorcycle);
        Task SaveChangesAsync();
        void Delete(Motorcycle motorcycle);
    }
}