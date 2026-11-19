using System.Text.Json.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BrainRush.Models.Entities;

/// <summary>View: leaderboard. One row per player, summed from the rounds history table.</summary>
[Table("leaderboard")]
public class LeaderboardRow : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("name")] public string Name { get; set; } = "";
    [Column("handle")] public string Handle { get; set; } = "";
    [Column("is_friend")] public bool IsFriend { get; set; }
    [Column("is_you")] public bool IsYou { get; set; }
    [Column("in_circle")] public bool InCircle { get; set; }
    [Column("streak")] public int Streak { get; set; }
    [Column("best_streak")] public int BestStreak { get; set; }
    [Column("accuracy")] public int Accuracy { get; set; }
    [Column("movies")] public int Movies { get; set; }
    [Column("music")] public int Music { get; set; }
    [Column("sports")] public int Sports { get; set; }
    [Column("gaming")] public int Gaming { get; set; }
    [Column("general")] public int General { get; set; }
    [Column("total")] public int Total { get; set; }

    [JsonIgnore] public string Initials => Avatar.Initials(Name);
    [JsonIgnore] public string AvatarCss => Avatar.Css(Id);

    /// <summary>Points for one category, or the total when no category is picked.</summary>
    public int ScoreFor(Category? c) => c switch
    {
        Category.Movies => Movies,
        Category.Music => Music,
        Category.Sports => Sports,
        Category.Gaming => Gaming,
        Category.General => General,
        _ => Total,
    };

    /// <summary>The view column to sort by for a category (used in Order on the server).</summary>
    public static string ScoreColumn(Category? c) => c?.ToString().ToLowerInvariant() ?? "total";
}
