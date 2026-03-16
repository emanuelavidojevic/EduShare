using EduShareBackend.Api.Models.Entities;

public interface IAdminService
{
    Task<IEnumerable<AdminDTO>> GetAllUsersAsync(string? search = null, int? roleId = null);
    Task<Dictionary<string, int>> GetDashboardStatsAsync();
    Task<AdminDTO> GetUserByIdAsync(int userId);
    Task<bool> UpdateUserAsync(int userId, AdminDTO userDto);
    Task<bool> DeleteUserAsync(int userId);
}