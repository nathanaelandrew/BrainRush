namespace BrainRush.Models.Entities;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Handle { get; set; } = "";
    public bool IsFriend { get; set; }
    public bool IsYou { get; set; }
    public int Streak { get; set; }
    public int BestStreak { get; set; }
    public int Accuracy { get; set; }
    public Dictionary<Category, int> Scores { get; set; } = new();

    public int Total => Scores.Values.Sum();

    public string Initials => string.Concat(
        Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(w => char.ToUpper(w[0])));

    public string AvatarCss => $"avatar-{Id % 3}";
}