using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Data;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduShareBackend.Api.Models;

namespace EduShareBackend.Api.Services.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly AplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public SubjectService(AplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<SubjectDTO>> GetNumberOfMaterial()
        {
            return await _context.Subjects
                .Include(s => s.User)
                .Select(s => new SubjectDTO
                {
                    SubjectId = s.SubjectId,
                    NameSubject = s.NameSubject,
                    DescriptionSubject = s.DescriptionSubject,
                    DepartmentId = s.DepartmentId,
                    MaterialCount = s.Materials.Count(),
                    // Pošto profesor mora da postoji, spajamo string direktno
                    ProfessorName = s.User != null 
                        ? "prof. " + s.User.FirstName + " " + s.User.LastName 
                        : "prof. nije dodeljen",
                    UserId = s.UserId
                })
                .ToListAsync();
        }

        public async Task<bool> CreateSubjectAsync(SubjectDTO dto)
        {
            var noviPredmet = new Subjects 
            {
                NameSubject = dto.NameSubject,
                DescriptionSubject = dto.DescriptionSubject,
                DepartmentId = dto.DepartmentId,
                UserId = dto.UserId // Dodela bez provere na null
            };

            _context.Subjects.Add(noviPredmet);
            var success = await _context.SaveChangesAsync() > 0;

            if (success)
            {
                await _notificationService.CreateNotification(
                    "Novi predmet", 
                    $"Dodat je predmet: {dto.NameSubject}", 
                    "SubjectCreated",
                    subjectId: noviPredmet.SubjectId
                );
            }

            return success;
        }

        public async Task<IEnumerable<SubjectDTO>> GetSubjectsByDepartmentAsync(int departmentId)
        {
            return await _context.Subjects
                .Where(s => s.DepartmentId == departmentId)
                .Include(s => s.User)
                .Select(s => new SubjectDTO
                {
                    SubjectId = s.SubjectId,
                    NameSubject = s.NameSubject,
                    DescriptionSubject = s.DescriptionSubject,
                    DepartmentId = s.DepartmentId,
                    MaterialCount = s.Materials.Count(),
                    ProfessorName = s.User != null 
                        ? "prof. " + s.User.FirstName + " " + s.User.LastName 
                        : "prof. nije dodeljen",
                    UserId = s.UserId
                })
                .ToListAsync();
        }
    }
}