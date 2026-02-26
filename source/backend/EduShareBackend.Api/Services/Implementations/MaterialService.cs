using EduShareBackend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Data;
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Models;
using EduShareBackend.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EduShareBackend.Api.Services.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly AplicationDbContext _context;

        public MaterialService(AplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaterialDTO>> TableMaterial()
        {
            return await _context.Materials
                .Include(m => m.User) 
                .OrderByDescending(m => m.DateCreated)
                .Take(5)
                .Select(m => new MaterialDTO
                {
                    MaterialId = m.MaterialId,
                    Name = m.Name,
                    DescriptionMaterial = m.DescriptionMaterial,
                    DateCreated = m.DateCreated,
                    Type = (MaterialType)m.Type, 
                    AuthorName = m.User != null 
                        ? $"{m.User.FirstName} {m.User.LastName}" 
                        : "Nepoznat Autor"
                })
                .ToListAsync();
        }

        public async Task<(byte[] Data, string FileName, string MimeType)?> MaterialDownload(int id)
        {
            var meta = await _context.Materials.FirstOrDefaultAsync(m => m.MaterialId == id);
            if (meta == null) return null;

            var fileData = await _context.MaterialData.FirstOrDefaultAsync(md => md.MaterialId == id);
            if (fileData == null) return null;

            string mimeType = (MaterialType)meta.Type switch
            {
                MaterialType.Pdf => "application/pdf",
                MaterialType.Image => "image/png",
                MaterialType.Video => "video/mp4",
                MaterialType.Document => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            return (fileData.Data, meta.Name, mimeType);
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
                    ".jpg" or ".jpeg" or ".png" or ".gif" => MaterialType.Image,
                    ".mp4" or ".avi" or ".mkv" => MaterialType.Video,
                    ".doc" or ".docx" or ".txt" => MaterialType.Document,
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
                    Name = dto.File.FileName,
                    DescriptionMaterial = (string.IsNullOrEmpty(dto.Description) || dto.Description == "string") 
                                          ? "Automatski dodat materijal" : dto.Description,
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

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null) return false;
            _context.Materials.Remove(material);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<object> GetProfileChartData(int userId)
        {
            var userMaterials = await _context.Materials
                .Include(m => m.Subject) 
                .Where(m => m.UserId == userId)
                .ToListAsync();

            if (userMaterials == null || !userMaterials.Any()) 
            {
                return new { types = new List<object>(), subjects = new List<object>(), totalCount = 0 };
            }

            var typeStats = userMaterials
                .GroupBy(m => (MaterialType)m.Type)
                .Select(g => new {
                    label = g.Key.ToString(), 
                    count = g.Count(),
                    percentage = Math.Round((double)g.Count() / userMaterials.Count * 100, 1)
                }).ToList();

            var subjectStats = userMaterials
                .GroupBy(m => m.Subject != null ? m.Subject.NameSubject : "Nepoznat predmet")
                .Select(g => new {
                    label = g.Key, 
                    count = g.Count(),
                    percentage = Math.Round((double)g.Count() / userMaterials.Count * 100, 1)
                })
                .OrderByDescending(x => x.count)
                .ToList();

            return new {
                types = typeStats,
                subjects = subjectStats,
                totalCount = userMaterials.Count
            };
        }
    }
}