using EduShareBackend.Api.Models.Entities;

namespace EduShareBackend.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<UserDTO> CreateAsync(UserDTO UserDTO);
        Task<string> AddRole(int UserId, int RoleId);
    }
}