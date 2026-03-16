using System.ComponentModel.DataAnnotations;

namespace EduShareBackend.Api.Models
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Mora biti validan email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [MinLength(5, ErrorMessage = "Lozinka mora imati najmanje 8 karaktera.")]
        public string Password { get; set; }

      [Required(ErrorMessage = "Departman je obavezan.")]
    [Range(1, int.MaxValue, ErrorMessage = "Izaberite validan departman.")]
    public int DepartmentId { get; set; }

    [Required(ErrorMessage = "Uloga je obavezna.")]
    [Range(1, int.MaxValue, ErrorMessage = "Izaberite validnu ulogu.")]
    public int RoleId { get; set; }
    }
}