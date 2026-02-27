using Microsoft.EntityFrameworkCore;
using EduShareBackend.Api.Data;
using EduShareBackend.Api.Models;
using EduShareBackend.Api.Models.Entities;
using EduShareBackend.Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduShareBackend.Api.Services.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly AplicationDbContext _context;

        public SubjectService(AplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectDTO>> GetNumberOfMaterial()
        {
            return await _context.Subjects
                .Select(s => new SubjectDTO
                {
                    SubjectId = s.SubjectId,
                    NameSubject = s.NameSubject,
                    DescriptionSubject = s.DescriptionSubject,
                    DepartmentId = s.DepartmentId,
                    MaterialCount = s.Materials.Count()
                })
                .ToListAsync();
        }

        public async Task<bool> CreateSubjectAsync(SubjectDTO dto)
        {
            var noviPredmet = new Subjects 
            {
                NameSubject = dto.NameSubject,
                DescriptionSubject = dto.DescriptionSubject,
                DepartmentId = dto.DepartmentId
            };

            _context.Subjects.Add(noviPredmet);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<SubjectDTO>> GetSubjectsByDepartmentAsync(int departmentId)
        {
            return await _context.Subjects
                .Where(s => s.DepartmentId == departmentId) 
                .Select(s => new SubjectDTO
                {
                    SubjectId = s.SubjectId,
                    NameSubject = s.NameSubject,
                    DescriptionSubject = s.DescriptionSubject,
                    DepartmentId = s.DepartmentId,
                    MaterialCount = s.Materials.Count()
                })
                .ToListAsync();
        }
    }
}