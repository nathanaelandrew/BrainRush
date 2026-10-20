namespace BrainRush.Models.Entities;

public class Question
{
    public int Id { get; set; }
    public Category Topic { get; set; }
    public string Prompt { get; set; } = "";
    public List<string> Options { get; set; } = new();
    public int CorrectIndex { get; set; }
    public int Seconds { get; set; } = 15;
    public int Points { get; set; } = 100;
}