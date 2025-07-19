using System.ComponentModel.DataAnnotations;

namespace MyMusicApp.Models; // Giữ cùng namespace cho đơn giản

public class PlaylistCreateViewModel
{
    [Required(ErrorMessage = "Tên Playlist không được để trống.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}