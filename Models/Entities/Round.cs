using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BrainRush.Models.Entities;

/// <summary>Table: rounds. History table: rows are only ever inserted, never updated or deleted.</summary>
[Table("rounds")]
public class Round : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("player_id")] public long PlayerId { get; set; }
    [Column("topic")] public string? TopicName { get; set; }          // null = mixed round
    [Column("challenge_id")] public long? ChallengeId { get; set; }
    [Column("score")] public int Score { get; set; }
    [Column("correct")] public int Correct { get; set; }
    [Column("total")] public int Total { get; set; }
    [Column("best_streak")] public int BestStreak { get; set; }
}

/// <summary>View: site_stats. Headline numbers for the home page in one row.</summary>
[Table("site_stats")]
public class SiteStats : BaseModel
{
    [Column("players")] public int Players { get; set; }
    [Column("best_streak")] public int BestStreak { get; set; }
    [Column("questions")] public int Questions { get; set; }
    [Column("open_challenges")] public int OpenChallenges { get; set; }
}
