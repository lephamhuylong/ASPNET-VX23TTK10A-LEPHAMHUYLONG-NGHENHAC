using System.ComponentModel.DataAnnotations;

namespace MyMusicApp.Models;

public class Feedback
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}