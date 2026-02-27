using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Data;
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Models;
using EduShareBackend.Api.Models.Enums;

namespace EduShareBackend.Api.Services.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly AplicationDbContext _context;

        public MaterialService(AplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaterialDTO>> TableMaterial(MaterialQueryDTO query)
        {
            var materialsQuery = _context.Materials
                .Include(m => m.User)
                .Include(m => m.Subject)
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
                    DescriptionMaterial = m.DescriptionMaterial,
                    DateCreated = m.DateCreated,
                    SubjectId = m.SubjectId,
                    NameSubject = m.Subject.NameSubject,
                    Type = (MaterialType)m.Type,
                    AuthorName = m.User != null ? $"{m.User.FirstName} {m.User.LastName}" : "Sistem"
                })
                .ToListAsync();
        }

        public async Task<bool> UploadMaterial(MaterialUploadDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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
                    // Ako je opis prazan ili "string", stavljamo null da ne ruži dizajn
                    DescriptionMaterial = (string.IsNullOrEmpty(dto.Description) || dto.Description == "string") 
                                          ? null : dto.Description,
                    UserId = dto.UserId, 
                    SubjectId = dto.SubjectId,
                    DateCreated = DateTime.Now,
                    Type = (int)finalType
                };

                _context.Materials.Add(newMaterial);
                await _context.SaveChangesAsync();

                _context.MaterialData.Add(new MaterialData { MaterialId = newMaterial.MaterialId, Data = fileBytes });
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch { await transaction.RollbackAsync(); return false; }
        }

        public async Task<object> GetProfileChartData(int userId)
        {
            var user = await _context.Users.Include(u => u.Department).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null || user.Department == null) return new { types = new List<object>(), subjects = new List<object>(), totalCount = 0 };

            var userMaterials = await _context.Materials.Where(m => m.UserId == userId).ToListAsync();
            
            var typeStats = userMaterials
                .GroupBy(m => (MaterialType)m.Type)
                .Select(g => new {
                    label = g.Key.ToString(),
                    count = g.Count(),
                    percentage = userMaterials.Any() ? Math.Round((double)g.Count() / userMaterials.Count * 100, 1) : 0
                }).ToList();

            var subjectsInDept = await _context.Subjects
                .Include(s => s.Materials)
                .Where(s => s.DepartmentId == user.DepartmentId)
                .ToListAsync();

            var totalMaterialsInDept = subjectsInDept.Sum(s => s.Materials.Count);

            var subjectStats = subjectsInDept
                .Select(s => new {
                    label = s.NameSubject,
                    count = s.Materials.Count,
                    percentage = totalMaterialsInDept > 0 ? Math.Round((double)s.Materials.Count / totalMaterialsInDept * 100, 1) : 0
                })
                .OrderByDescending(x => x.count).Take(6).ToList();

            return new { types = typeStats, subjects = subjectStats, totalCount = userMaterials.Count };
        }

        public async Task<(byte[] Data, string FileName, string MimeType)?> MaterialDownload(int id)
        {
            var meta = await _context.Materials.FirstOrDefaultAsync(m => m.MaterialId == id);
            var fileData = await _context.MaterialData.FirstOrDefaultAsync(md => md.MaterialId == id);
            if (meta == null || fileData == null) return null;

            string mimeType = (MaterialType)meta.Type switch {
                MaterialType.Pdf => "application/pdf",
                MaterialType.Image => "image/png",
                MaterialType.Document => "application/msword",
                _ => "application/octet-stream"
            };
            return (fileData.Data, meta.Name, mimeType);
        }

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null) return false;
            _context.Materials.Remove(material);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}