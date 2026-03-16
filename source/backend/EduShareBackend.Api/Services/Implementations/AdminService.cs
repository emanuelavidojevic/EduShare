using EduShareBackend.Api.Data;
using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EduShareBackend.Api.Services
{
    public class AdminService : IAdminService
    {
        private readonly AplicationDbContext _context;

        public AdminService(AplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminDTO>> GetAllUsersAsync(string? search = null, int? roleId = null)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.Department)
                .AsQueryable();

            // Filtriranje po pretrazi (Ime, Prezime, Email)
            if (!string.IsNullOrEmpty(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(lowerSearch) || 
                    u.LastName.ToLower().Contains(lowerSearch) || 
                    u.Email.ToLower().Contains(lowerSearch));
            }

            // Filtriranje po ulozi (Admin, Professor, Student)
            if (roleId.HasValue)
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId.Value));
            }

            var users = await query.ToListAsync();

            return users.Select(u => new AdminDTO
            {
                UserId = u.UserId,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                RoleName = u.UserRoles?.FirstOrDefault()?.Role?.RoleName ?? "Nema ulogu",
                DepartmentName = u.Department?.DepartmentName ?? "Nema odeljenje",
                DateRegistration = u.DateRegistration
            });
        }

        public async Task<Dictionary<string, int>> GetDashboardStatsAsync()
        {
            var stats = new Dictionary<string, int>();

            stats.Add("TotalUsers", await _context.Users.CountAsync());
            stats.Add("TotalMaterials", await _context.Materials.CountAsync());
            stats.Add("TotalSubjects", await _context.Subjects.CountAsync());
            
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            stats.Add("NewUsersMonth", await _context.Users.CountAsync(u => u.DateRegistration >= thirtyDaysAgo));

            return stats;
        }

        public async Task<AdminDTO> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return null;

            return new AdminDTO
            {
                UserId = user.UserId,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                RoleName = user.UserRoles?.FirstOrDefault()?.Role?.RoleName,
                DepartmentName = user.Department?.DepartmentName,
                DateRegistration = user.DateRegistration
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(int userId, AdminDTO userDto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return false;

            // 1. Azuriranje email-a
            user.Email = userDto.Email;

            // 2. Mapiranje FullName nazad u FirstName i LastName
            if (!string.IsNullOrWhiteSpace(userDto.FullName))
            {
                var names = userDto.FullName.Trim().Split(' ', 2);
                user.FirstName = names[0];
                user.LastName = names.Length > 1 ? names[1] : "";
            }

            // 3. Azuriranje uloge
            if (!string.IsNullOrEmpty(userDto.RoleName))
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == userDto.RoleName);
                if (role != null)
                {
                    // Uklanjamo trenutne uloge
                    var currentRoles = _context.UserRoles.Where(ur => ur.UserId == userId);
                    _context.UserRoles.RemoveRange(currentRoles);

                    // Dodajemo novu odabranu ulogu
                    _context.UserRoles.Add(new UserRoles
                    { 
                        UserId = userId, 
                        RoleId = role.RoleId 
                    });
                }
            }
            
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}