using Microsoft.AspNetCore.Mvc;
using EduShareBackend.Api.Services.Interfaces;
using EduShareBackend.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace EduShareBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService=subjectService;
        }
        [HttpGet]
public async Task<IActionResult> GetSubjects()
{
    var subjects = await _subjectService.GetNumberOfMaterial();
    return Ok(subjects);
}


[HttpPost]
public async Task<IActionResult> AddSubject([FromBody] SubjectDTO subjectDto)
{
    if (subjectDto == null) return BadRequest("Podaci nisu validni.");

    var uspeh = await _subjectService.CreateSubjectAsync(subjectDto);

    if (uspeh)
    {
        return Ok(new { poruka = "Predmet uspešno dodat!" });
    }

    return BadRequest("Došlo je do greške pri čuvanju predmeta.");
}
    }
}