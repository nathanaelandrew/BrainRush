using System.ComponentModel.DataAnnotations;
using BrainRush.Models.Entities;

namespace BrainRush.Models.Forms;

public class ChallengeForm
{
    [Required(ErrorMessage = "Name the challenge so friends know what they are joining.")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Use 3 to 40 characters for the name.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Pick a topic for today's questions.")]
    public Category? Topic { get; set; }

    [Range(3, 10, ErrorMessage = "Choose between 3 and 10 questions.")]
    public int QuestionCount { get; set; } = 5;

    [Range(10, 30, ErrorMessage = "Time per question must be 10 to 30 seconds.")]
    public int SecondsPerQuestion { get; set; } = 15;

    [MinLength(1, ErrorMessage = "Invite at least one friend.")]
    [MaxLength(6, ErrorMessage = "Invite six friends at most.")]
    public List<string> Friends { get; set; } = new();

    [StringLength(120, ErrorMessage = "Keep the trash talk under 120 characters.")]
    public string? Note { get; set; }
}