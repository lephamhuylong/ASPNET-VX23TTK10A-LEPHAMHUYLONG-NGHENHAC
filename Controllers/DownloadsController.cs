using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMusicApp.Data;
using MyMusicApp.Models;
using System.Security.Claims;

namespace MyMusicApp.Controllers
{
    [Authorize] // Yêu cầu người dùng phải đăng nhập để tải nhạc
    public class DownloadsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DownloadsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Downloads/Song/5
        public async Task<IActionResult> Song(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null || string.IsNullOrEmpty(song.FilePath))
            {
                return NotFound();
            }

            // Tăng biến đếm lượt tải
            song.DownloadCount++;

            // Ghi lại nhật ký tải
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var downloadLog = new DownloadLog
            {
                SongId = song.Id,
                UserId = userId,
                DownloadTime = DateTime.UtcNow
            };
            _context.DownloadLogs.Add(downloadLog);

            await _context.SaveChangesAsync();

            // Lấy đường dẫn vật lý của file
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, song.FilePath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Đọc file và trả về cho người dùng
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            // Trả về file với tên gốc để trình duyệt hiển thị đúng
            return File(memory, "application/octet-stream", Path.GetFileName(filePath));
        }
    }
}
