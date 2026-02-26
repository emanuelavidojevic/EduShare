using EduShareBackend.Api.Services.Interfaces;
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
    public class DepartmentService : IDepartmentService
    {
        private readonly AplicationDbContext _context;

        public DepartmentService(AplicationDbContext context)
        {
            _context = context;
        }

    }
}