using System.ComponentModel.DataAnnotations;

namespace MyMusicApp.Models;

public class Genre
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên thể loại không được để trống.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Song>? Songs { get; set; }
}