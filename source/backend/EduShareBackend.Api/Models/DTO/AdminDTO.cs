namespace EduShareBackend.Api.Models.Entities
{
    public class AdminDTO
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
    public string DepartmentName { get; set; }
    public DateTime DateRegistration { get; set; }
}
}