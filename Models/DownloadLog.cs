using System.ComponentModel.DataAnnotations;

namespace MyMusicApp.Models;

public class DownloadLog
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public int SongId { get; set; }
    public virtual Song Song { get; set; }

    public DateTime DownloadTime { get; set; } = DateTime.Now;
}