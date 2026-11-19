using System.Text.Json.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BrainRush.Models.Entities;

/// <summary>Table: challenges. Soft delete: cancelling fills in deleted_at.</summary>
[Table("challenges")]
public class DailyChallenge : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("title")] public string Title { get; set; } = "";
    [Column("topic")] public string TopicName { get; set; } = "";
    [Column("host_id")] public long HostId { get; set; }
    [Column("question_count")] public int QuestionCount { get; set; } = 5;
    [Column("seconds")] public int Seconds { get; set; } = 15;
    [Column("note")] public string? Note { get; set; }
    [Column("deleted_at")] public DateTime? DeletedAt { get; set; }
}

/// <summary>Table: challenge_players. Who was invited, and when they finished.</summary>
[Table("challenge_players")]
public class ChallengePlayer : BaseModel
{
    [PrimaryKey("challenge_id", true)] public long ChallengeId { get; set; }
    [PrimaryKey("player_id", true)] public long PlayerId { get; set; }
    [Column("finished_at")] public DateTime? FinishedAt { get; set; }
}

/// <summary>View: challenge_cards. A live challenge with its host and members in one row.</summary>
[Table("challenge_cards")]
public class ChallengeCard : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("title")] public string Title { get; set; } = "";
    [Column("topic")] public string TopicName { get; set; } = "";
    [Column("question_count")] public int QuestionCount { get; set; }
    [Column("seconds")] public int Seconds { get; set; }
    [Column("note")] public string? Note { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; }
    [Column("host_id")] public long HostId { get; set; }
    [Column("host_name")] public string HostName { get; set; } = "";
    [Column("host_handle")] public string HostHandle { get; set; } = "";
    [Column("members")] public List<ChallengeMember> Members { get; set; } = new();

    [JsonIgnore] public Category Arena => TopicName.ToCategory();
    [JsonIgnore] public int FinishedCount => Members.Count(m => m.Finished);
    [JsonIgnore] public IEnumerable<ChallengeMember> Waiting => Members.Where(m => !m.Finished);
    public bool Includes(long playerId) => Members.Any(m => m.Id == playerId);
    public bool HasFinished(long playerId) => Members.Any(m => m.Id == playerId && m.Finished);
}

/// <summary>One entry of challenge_cards.members (built by jsonb_build_object in the view).</summary>
public class ChallengeMember
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("handle")] public string Handle { get; set; } = "";
    [JsonPropertyName("finished")] public bool Finished { get; set; }

    [JsonIgnore] public string Initials => Avatar.Initials(Name);
    [JsonIgnore] public string AvatarCss => Avatar.Css(Id);
}
