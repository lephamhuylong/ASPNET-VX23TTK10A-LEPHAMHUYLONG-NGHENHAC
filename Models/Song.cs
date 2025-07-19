namespace MyMusicApp.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string FilePath { get; set; }
        public string? ImageUrl { get; set; }
        public int ListenCount { get; set; } = 0;
        public int DownloadCount { get; set; } = 0;

        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
