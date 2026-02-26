using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Data;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Services.Interfaces;

namespace EduShareBackend.Api.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AplicationDbContext _context;

        public UserService(AplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                DateRegistration = u.DateRegistration
            });
        }

        public async Task<UserDTO> CreateAsync(UserDTO UserDTO)
        {
            string securePassword = BCrypt.Net.BCrypt.HashPassword(UserDTO.Password);
            var newUser = new Users
            {
                FirstName = UserDTO.FirstName,
                LastName = UserDTO.LastName,
                Email = UserDTO.Email,
                Password = securePassword,
                SecurityStamp = Guid.NewGuid().ToString(),
                DateRegistration = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                UserId = newUser.UserId,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                DateRegistration = newUser.DateRegistration
            };
        }

        public async Task<string> AddRole(int userId, int roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return "Nema korisnika u sistemu";

            await _context.Entry(user).Collection(u => u.UserRoles).LoadAsync();
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return "Nema uloga u sistemu";

            if (user.UserRoles.Any(ur => ur.RoleId == roleId))
                return "Korisnik već ima tu ulogu.";

            foreach (var ur in user.UserRoles)
            {
                await _context.Entry(ur).Reference(x => x.Role).LoadAsync();
            }

            bool isStudent = user.UserRoles.Any(ur => ur.Role.RoleName == "Student");
            if (isStudent && role.RoleName == "Admin")
            {
                return "Error! Student ne moze biti admin";
            }

            user.UserRoles.Add(new UserRoles { UserId = userId, RoleId = roleId });
            user.SecurityStamp = Guid.NewGuid().ToString();

            await _context.SaveChangesAsync();
            return "Uloga je uspešno dodeljena!";
        }
    }
}