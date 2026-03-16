using EduShareBackend.Api.Models;

namespace EduShareBackend.Api.Services.Interfaces
{
    public interface IRatingService
    {
        Task AddOrUpdate(int userId, int materialId, int value);
        Task<double> GetAverage(int materialId);
    }
}