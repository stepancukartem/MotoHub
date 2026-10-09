
using Microsoft.EntityFrameworkCore;
using MotoHub.DAL;
using MotoHub.DAL.Entities;
using MotoHub.DTOs;

namespace MotoHub.Services
{
    public class ReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewDto>> GetByMotorcycleAsync(
            int motorcycleId)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.MotorcycleId == motorcycleId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    MotorcycleId = r.MotorcycleId,
                    MotorcycleName = r.Motorcycle.Name,
                    UserId = r.UserId,
                    UserEmail = r.User.Email,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ReviewDto?> CreateAsync(
            int userId,
            CreateReviewDto dto)
        {
            var comment = dto.Comment?.Trim();

            if (dto.Rating < 1 || dto.Rating > 5 ||
                string.IsNullOrWhiteSpace(comment) ||
                comment.Length > 1000)
            {
                return null;
            }

            var motorcycleExists = await _context.Motorcycles
                .AnyAsync(m => m.Id == dto.MotorcycleId);

            if (!motorcycleExists)
                return null;

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r =>
                    r.UserId == userId &&
                    r.MotorcycleId == dto.MotorcycleId);

            if (alreadyReviewed)
                return null;

            var review = new Review
            {
                UserId = userId,
                MotorcycleId = dto.MotorcycleId,
                Rating = dto.Rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.Id == review.Id)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    MotorcycleId = r.MotorcycleId,
                    MotorcycleName = r.Motorcycle.Name,
                    UserId = r.UserId,
                    UserEmail = r.User.Email,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAsync(
            int reviewId,
            int userId,
            bool isAdmin)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return false;

            if (!isAdmin && review.UserId != userId)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
