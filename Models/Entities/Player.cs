using System.Text.Json.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BrainRush.Models.Entities;

/// <summary>Table: players. Streak columns are overwritten after each round.</summary>
[Table("players")]
public class Player : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("handle")]
    public string Handle { get; set; } = "";

    [Column("is_friend")]
    public bool IsFriend { get; set; }

    [Column("is_you")]
    public bool IsYou { get; set; }

    [Column("streak")]
    public int Streak { get; set; }

    [Column("best_streak")]
    public int BestStreak { get; set; }

    [JsonIgnore] public string Initials => Avatar.Initials(Name);
    [JsonIgnore] public string AvatarCss => Avatar.Css(Id);
}
