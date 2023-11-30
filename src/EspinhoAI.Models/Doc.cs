namespace EspinhoAI.Models;

public class Doc
{
    public int Id { get; set; }
    public string? Url { get; set; }
    public string? Path { get; set; }
    public string? Publication { get; set; }
    public string? Year { get; set; }
    public string? Month { get; set; }
    public string? Day { get; set; }
    public string? FileName { get; set; }
    public DateTime ScrapDate { get; set; }
}
