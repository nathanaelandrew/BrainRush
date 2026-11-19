using System.ComponentModel.DataAnnotations;
using BrainRush.Models.Entities;

namespace BrainRush.Models.Forms;

/// <summary>Add/edit form for the question bank. The same limits are CHECK constraints in the database.</summary>
public class QuestionForm
{
    [Required(ErrorMessage = "Pick the arena this question belongs to.")]
    public Category? Topic { get; set; }

    [Required(ErrorMessage = "Write the question.")]
    [StringLength(200, MinimumLength = 10, ErrorMessage = "Use 10 to 200 characters for the question.")]
    public string Prompt { get; set; } = "";

    [Required(ErrorMessage = "Every question needs answer A.")] public string OptionA { get; set; } = "";
    [Required(ErrorMessage = "Every question needs answer B.")] public string OptionB { get; set; } = "";
    [Required(ErrorMessage = "Every question needs answer C.")] public string OptionC { get; set; } = "";
    [Required(ErrorMessage = "Every question needs answer D.")] public string OptionD { get; set; } = "";

    [Range(0, 3, ErrorMessage = "Mark which answer is right.")]
    public int CorrectIndex { get; set; }

    [Range(10, 30, ErrorMessage = "Time per question must be 10 to 30 seconds.")]
    public int Seconds { get; set; } = 15;

    public List<string> Options => new() { OptionA.Trim(), OptionB.Trim(), OptionC.Trim(), OptionD.Trim() };

    public static QuestionForm From(Question q) => new()
    {
        Topic = q.Arena,
        Prompt = q.Prompt,
        OptionA = q.Options.ElementAtOrDefault(0) ?? "",
        OptionB = q.Options.ElementAtOrDefault(1) ?? "",
        OptionC = q.Options.ElementAtOrDefault(2) ?? "",
        OptionD = q.Options.ElementAtOrDefault(3) ?? "",
        CorrectIndex = q.CorrectIndex,
        Seconds = q.Seconds,
    };
}
