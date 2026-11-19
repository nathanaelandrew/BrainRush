namespace BrainRush.Models.Entities;

/// <summary>The five arenas. Stored in Postgres as the enum type public.category.</summary>
public enum Category { Movies, Music, Sports, Gaming, General }

public static class CategoryExtensions
{
    public static string Label(this Category c) => c == Category.General ? "General knowledge" : c.ToString();

    public static string Css(this Category c) => "tag-" + c.ToString().ToLowerInvariant();

    public static string Blurb(this Category c) => c switch
    {
        Category.Movies => "Oscars, directors, and scenes you can quote.",
        Category.Music => "Albums, instruments, and chart history.",
        Category.Sports => "Rules, records, and final whistles.",
        Category.Gaming => "Consoles, worlds, and boss fights.",
        _ => "Science, geography, and everything between.",
    };

    /// <summary>Turns the text from a topic column back into a Category.</summary>
    public static Category ToCategory(this string? topic) =>
        Enum.TryParse<Category>(topic, true, out var c) ? c : Category.General;
}

/// <summary>Small display helpers shared by every model that shows a person.</summary>
public static class Avatar
{
    public static string Initials(string name) => string.Concat(
        name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(w => char.ToUpper(w[0])));

    public static string Css(long id) => $"avatar-{id % 3}";
}
