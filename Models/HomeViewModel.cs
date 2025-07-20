namespace MyMusicApp.Models
{
    
    public class HomeViewModel
    {
        public List<Song> LatestSongs { get; set; }
        public List<Song> ChartSongs { get; set; }
        public List<Song> FeaturedPlaylists { get; set; } 

        public HomeViewModel()
        {
            LatestSongs = new List<Song>();
            ChartSongs = new List<Song>();
            FeaturedPlaylists = new List<Song>();
        }
    }
}
