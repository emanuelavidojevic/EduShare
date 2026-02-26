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
                .Select(m => new MaterialDTO
                {
                    MaterialId = m.MaterialId,
                    Name = m.Name,
                    DescriptionMaterial = m.DescriptionMaterial,
                    DateCreated = m.DateCreated,
                    Type = (MaterialType)m.Type
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
    MaterialType.Document => "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // Za .docx
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
                    ".mp4" or ".avi" or ".mkv" => MaterialType.Video,
                    ".jpg" or ".jpeg" or ".png" => MaterialType.Image,
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

        // 5. Implementacija DeleteMaterialAsync
        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null) return false;
            _context.Materials.Remove(material);
            return await _context.SaveChangesAsync() > 0;
        }
public async Task<IEnumerable<UserMaterialStatsDTO>> GetUserMaterialStats(int userId)
{
    var userMaterials = await _context.Materials
        .Where(m => m.UserId == userId)
        .ToListAsync();

    int total = userMaterials.Count;
    if (total == 0) return new List<UserMaterialStatsDTO>();

    return userMaterials
        .GroupBy(m => (MaterialType)m.Type)
        .Select(g => new UserMaterialStatsDTO
        {
            TypeName = g.Key.ToString(),
            Count = g.Count(),
            Percentage = Math.Round((double)g.Count() / total * 100, 1)
        })
        .ToList();
}

public async Task<List<string>> GetUserImagesAsync(int userId)
{
    var imageIds = await _context.Materials
        .Where(m => m.UserId == userId && m.Type == (int)MaterialType.Image)
        .Select(m => m.MaterialId)
        .ToListAsync();

    var imageData = await _context.MaterialData
        .Where(md => imageIds.Contains(md.MaterialId))
        .Select(md => md.Data)
        .ToListAsync();

    return imageData
        .Select(data => $"data:image/png;base64,{Convert.ToBase64String(data)}")
        .ToList();
}
    }
}