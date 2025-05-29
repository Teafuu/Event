namespace Domain.Models;

public class Event
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageText { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public string? Audience { get; set; }
    public string? FilterList { get; set; }
    public string? Location { get; set; }
    public string? Language { get; set; }
}
