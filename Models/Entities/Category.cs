namespace BrainRush.Models.Entities;

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
}