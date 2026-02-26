using System.ComponentModel.DataAnnotations;

namespace EduShareBackend.Api.Models
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Mora biti validan email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [MinLength(8, ErrorMessage = "Lozinka mora imati najmanje 8 karaktera.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}