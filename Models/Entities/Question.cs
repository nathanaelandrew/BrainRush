using System.Text.Json.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BrainRush.Models.Entities;

/// <summary>Table: questions. Soft delete: Remove fills in deleted_at, the row stays.</summary>
[Table("questions")]
public class Question : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("topic")]
    public string TopicName { get; set; } = nameof(Category.General);

    [Column("prompt")]
    public string Prompt { get; set; } = "";

    [Column("options")]
    public List<string> Options { get; set; } = new();

    [Column("correct_index")]
    public int CorrectIndex { get; set; }

    [Column("seconds")]
    public int Seconds { get; set; } = 15;

    [Column("points")]
    public int Points { get; set; } = 100;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
    public Category Arena
    {
        get => TopicName.ToCategory();
        set => TopicName = value.ToString();
    }
}

/// <summary>View: question_pool. Live questions in random order, for starting a round.</summary>
[Table("question_pool")]
public class PoolQuestion : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("topic")] public string TopicName { get; set; } = "";
    [Column("prompt")] public string Prompt { get; set; } = "";
    [Column("options")] public List<string> Options { get; set; } = new();
    [Column("correct_index")] public int CorrectIndex { get; set; }
    [Column("seconds")] public int Seconds { get; set; } = 15;
    [Column("points")] public int Points { get; set; } = 100;

    [JsonIgnore] public Category Arena => TopicName.ToCategory();
}

/// <summary>View: question_counts. Live questions per topic.</summary>
[Table("question_counts")]
public class QuestionCount : BaseModel
{
    [Column("topic")] public string TopicName { get; set; } = "";
    [Column("questions")] public int Questions { get; set; }
}
