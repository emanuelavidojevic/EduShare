using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Data;
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Models;
using EduShareBackend.Api.Models.Enums;
using System.Security.Claims;

namespace EduShareBackend.Api.Services.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly AplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public MaterialService(AplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<MaterialDTO>> TableMaterial(MaterialQueryDTO query)
        {
            var materialsQuery = _context.Materials
                .Include(m => m.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .Include(m => m.Subject)
                    .ThenInclude(s => s.User)
                .Include(m => m.Rating)
                .AsQueryable();

            if (query.SubjectId.HasValue)
                materialsQuery = materialsQuery.Where(m => m.SubjectId == query.SubjectId);

            if (query.UserId.HasValue)
                materialsQuery = materialsQuery.Where(m => m.UserId == query.UserId);

            if (!string.IsNullOrEmpty(query.SearchTerm))
                materialsQuery = materialsQuery.Where(m => m.Name.Contains(query.SearchTerm));

            materialsQuery = query.SortBy == "name" 
                ? materialsQuery.OrderBy(m => m.Name) 
                : materialsQuery.OrderByDescending(m => m.DateCreated);

            return await materialsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new MaterialDTO
                {
                    MaterialId = m.MaterialId,
                    Name = m.Name,
                    UserId = m.UserId,
                    DescriptionMaterial = m.DescriptionMaterial,
                    DateCreated = m.DateCreated,
                    SubjectId = m.SubjectId,
                    NameSubject = m.Subject.NameSubject,
                    ProfessorName = m.Subject.User != null 
                        ? "prof. " + m.Subject.User.FirstName + " " + m.Subject.User.LastName 
                        : "Nema profesora",
                    Type = (MaterialType)m.Type,
                    AuthorName = m.User != null 
                        ? (m.User.UserRoles.Any(ur => ur.Role.RoleName == "Professor") 
                            ? "prof. " + m.User.FirstName + " " + m.User.LastName 
                            : m.User.FirstName + " " + m.User.LastName)
                        : "Sistem",
                    AverageRating = m.Rating.Any() ? Math.Round(m.Rating.Average(r => r.ValueForRatings), 1) : 0
                })
                .ToListAsync();
        }

        public async Task<bool> UploadMaterial(MaterialUploadDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

                if (user == null) return false;

                string displayAuthor = user.UserRoles.Any(ur => ur.Role.RoleName == "Professor") 
                    ? "prof. " + user.FirstName + " " + user.LastName 
                    : user.FirstName + " " + user.LastName;

                string extension = Path.GetExtension(dto.File.FileName).ToLower();
                MaterialType finalType = extension switch
                {
                    ".pdf" => MaterialType.Pdf,
                    ".jpg" or ".jpeg" or ".png" => MaterialType.Image,
                    ".mp4" or ".avi" => MaterialType.Video,
                    ".doc" or ".docx" => MaterialType.Document,
                    _ => MaterialType.Others
                };

                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await dto.File.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                var newMaterial = new Materials
                {
                    Name = dto.Name,
                    // FIX: Prosleđujemo prazan string umesto null jer baza ne dozvoljava null u ovoj koloni
                    DescriptionMaterial = (string.IsNullOrEmpty(dto.Description) || dto.Description == "string") 
                                          ? "" 
                                          : dto.Description,
                    UserId = dto.UserId, 
                    SubjectId = dto.SubjectId,
                    DateCreated = DateTime.UtcNow,
                    Type = (int)finalType
                };

                _context.Materials.Add(newMaterial);
                await _context.SaveChangesAsync();

                _context.MaterialData.Add(new MaterialData { MaterialId = newMaterial.MaterialId, Data = fileBytes });
                await _context.SaveChangesAsync();

                await _notificationService.CreateNotification(
                    title: "Novi materijal",
                    message: $"{displayAuthor} je dodao novi fajl: {newMaterial.Name}",
                    type: "Upload",
                    subjectId: newMaterial.SubjectId,
                    userId: newMaterial.UserId,
                    materialId: newMaterial.MaterialId 
                );

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            { 
                await transaction.RollbackAsync();
                return false; 
            }
        }

        public async Task<bool> DeleteMaterialAsync(int id, int currentUserId, string currentUserRole)
        {
            var material = await _context.Materials
                .Include(m => m.User)
                .ThenInclude(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(m => m.MaterialId == id);

            if (material == null) return false;

            if (currentUserRole == "Admin")
            {
                _context.Materials.Remove(material);
                return await _context.SaveChangesAsync() > 0;
            }

            if (currentUserRole == "Professor")
            {
                bool isOwner = material.UserId == currentUserId;
                bool ownerIsStudent = material.User.UserRoles.Any(ur => ur.Role.RoleName == "Student");

                if (isOwner || ownerIsStudent)
                {
                    _context.Materials.Remove(material);
                    return await _context.SaveChangesAsync() > 0;
                }
            }

            if (currentUserRole == "Student" && material.UserId == currentUserId)
            {
                _context.Materials.Remove(material);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<object> GetProfileChartData(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.Department == null) 
                return new { types = new List<object>(), subjects = new List<object>(), totalCount = 0 };

            var userMaterials = await _context.Materials
                .Where(m => m.UserId == userId)
                .ToListAsync();
            
            var typeStats = userMaterials
                .GroupBy(m => (MaterialType)m.Type)
                .Select(g => new {
                    label = g.Key.ToString(),
                    count = g.Count(),
                    percentage = userMaterials.Any() ? Math.Round((double)g.Count() / userMaterials.Count * 100, 1) : 0
                }).ToList();

            var subjectsInDept = await _context.Subjects
                .Where(s => s.DepartmentId == user.DepartmentId)
                .Select(s => new {
                    s.SubjectId,
                    s.NameSubject
                })
                .ToListAsync();

            var subjectStats = new List<object>();
            foreach (var s in subjectsInDept)
            {
                var count = userMaterials.Count(m => m.SubjectId == s.SubjectId);
                if (count > 0)
                {
                    subjectStats.Add(new {
                        label = s.NameSubject,
                        count = count,
                        percentage = userMaterials.Any() ? Math.Round((double)count / userMaterials.Count * 100, 1) : 0
                    });
                }
            }

            return new { 
                types = typeStats, 
                subjects = subjectStats.OrderByDescending(x => ((dynamic)x).count).Take(6).ToList(), 
                totalCount = userMaterials.Count 
            };
        }

        public async Task<(byte[] Data, string FileName, string MimeType)?> MaterialDownload(int id)
        {
            var meta = await _context.Materials.FirstOrDefaultAsync(m => m.MaterialId == id);
            var fileData = await _context.MaterialData.FirstOrDefaultAsync(md => md.MaterialId == id);
            if (meta == null || fileData == null) return null;

            string mimeType = (MaterialType)meta.Type switch {
                MaterialType.Pdf => "application/pdf",
                MaterialType.Image => "image/png",
                MaterialType.Video => "video/mp4",
                MaterialType.Document => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "text/plain" 
            };

            return (fileData.Data, meta.Name, mimeType);
        }
    }
}