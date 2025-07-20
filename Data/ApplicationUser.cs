using Microsoft.AspNetCore.Identity;
using MyMusicApp.Models; 

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }

  
    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
