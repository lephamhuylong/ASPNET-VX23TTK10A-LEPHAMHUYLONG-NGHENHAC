namespace MyMusicApp.Models
{
    // Lớp này dùng để chứa tất cả dữ liệu cần thiết cho trang chủ
    public class HomeViewModel
    {
        public List<Song> LatestSongs { get; set; }
        public List<Song> ChartSongs { get; set; }
        public List<Song> FeaturedPlaylists { get; set; } // Tạm thời dùng Song để làm placeholder

        public HomeViewModel()
        {
            LatestSongs = new List<Song>();
            ChartSongs = new List<Song>();
            FeaturedPlaylists = new List<Song>();
        }
    }
}
