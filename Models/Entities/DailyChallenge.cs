namespace BrainRush.Models.Entities;

public class DailyChallenge
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public Category Topic { get; set; }
    public string Host { get; set; } = "";
    public int Questions { get; set; } = 5;
    public int Seconds { get; set; } = 15;
    public string? Note { get; set; }
    public List<string> Invited { get; set; } = new();
    public List<string> Finished { get; set; } = new();
}