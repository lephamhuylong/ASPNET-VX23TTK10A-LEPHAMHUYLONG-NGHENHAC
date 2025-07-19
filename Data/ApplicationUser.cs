using Microsoft.AspNetCore.Identity;
using MyMusicApp.Models; // Thêm dòng này

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }

    // THÊM DÒNG NÀY ĐỂ HOÀN THIỆN MỐI QUAN HỆ
    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
