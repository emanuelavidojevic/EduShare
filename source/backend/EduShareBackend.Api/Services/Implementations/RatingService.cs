using EduShareBackend.Api.Data;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduShareBackend.Api.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly AplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public RatingService(AplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task AddOrUpdate(int userId, int materialId, int value)
        {
            var material = await _context.Materials.FindAsync(materialId);
            var user = await _context.Users.FindAsync(userId);
            string authorName = user != null ? $"{user.FirstName} {user.LastName}" : "Neko";

            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MaterialId == materialId);

            if (existingRating != null)
            {
                existingRating.ValueForRatings = value;
            }
            else
            {
                _context.Ratings.Add(new Ratings
                {
                    UserId = userId,
                    MaterialId = materialId,
                    ValueForRatings = value
                });
            }

            await _context.SaveChangesAsync();

            if (material != null && material.UserId != userId)
            {
                await _notificationService.CreateNotification(
                    title: "Nova ocena",
                    message: $"{authorName} je ocenio tvoj materijal '{material.Name}' sa {value} zvezdica.",
                    type: "Rating",
                    subjectId: material.SubjectId,
                    userId: userId,
                    materialId: materialId
                );
            }
        }

        public async Task<double> GetAverage(int materialId)
        {
            var average = await _context.Ratings
                .Where(r => r.MaterialId == materialId)
                .Select(r => (double?)r.ValueForRatings)
                .AverageAsync();

            return average.HasValue ? Math.Round(average.Value, 1) : 0;
        }
    }
}