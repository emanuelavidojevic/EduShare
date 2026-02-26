using EduShareBackend.Api.Models;
using Microsoft.EntityFrameworkCore; //za komande
using EduShareBackend.Api.Data; //db
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Services.Implementations;
using EduShareBackend.Api.Models.Entities;


using EduShareBackend.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EduShareBackend.Api.Services.Interfaces
{
    public interface ISubjectService
    {
    Task<IEnumerable<SubjectDTO>>GetNumberOfMaterial();
    Task<bool> CreateSubjectAsync(SubjectDTO dto);
    }
}