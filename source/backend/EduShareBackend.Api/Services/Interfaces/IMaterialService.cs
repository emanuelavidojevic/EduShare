using EduShareBackend.Api.Models;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Data;
using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Services.Implementations;
using EduShareBackend.Api.Services.Interfaces;


namespace EduShareBackend.Api.Services.Interfaces
{

    public interface IMaterialService{
    Task<bool>UploadMaterial(MaterialUploadDTO materialUploadDTO);
    Task<IEnumerable<MaterialDTO>> TableMaterial(MaterialQueryDTO query);
    Task<(byte[] Data, string FileName, string MimeType)?> MaterialDownload(int id);
    Task<object> GetProfileChartData(int userId);
    Task<bool> DeleteMaterialAsync(int id, int currentUserId, string currentUserRole);
    }
}