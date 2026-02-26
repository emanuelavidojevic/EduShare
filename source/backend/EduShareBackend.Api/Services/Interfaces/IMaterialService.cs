using EduShareBackend.Api.Models;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Data;
using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Services.Implementations;
using EduShareBackend.Api.Services.Interfaces;


namespace EduShareBackend.Api.Services.Interfaces
{

    public interface IMaterialService{
    Task<bool>DeleteMaterialAsync(int id);//obrisi materijal
    Task<bool>UploadMaterial(MaterialUploadDTO materialUploadDTO);
    Task<IEnumerable<MaterialDTO>> TableMaterial();    
    Task<(byte[] Data, string FileName, string MimeType)?> MaterialDownload(int id);
    Task<IEnumerable<UserMaterialStatsDTO>> GetUserMaterialStats(int userId);
    Task<List<string>> GetUserImagesAsync(int userId);
}
}