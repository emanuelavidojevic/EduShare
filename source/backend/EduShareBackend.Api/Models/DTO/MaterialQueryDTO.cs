public class MaterialQueryDTO
{
    public int? SubjectId { get; set; }
    public int? UserId { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "date"; // date, name
}