using System.ComponentModel.DataAnnotations;
using MyMusicApp.Models;

namespace MyMusicApp.Areas.Admin.Models;

public class SongViewModel
{
    [Required(ErrorMessage = "Tên bài hát không được để trống")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên nghệ sĩ không được để trống")]
    public string Artist { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn thể loại")]
    public int GenreId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn file nhạc")]
    public IFormFile? MusicFile { get; set; }

    public IFormFile? ImageFile { get; set; }

    public IEnumerable<Genre>? Genres { get; set; }
}