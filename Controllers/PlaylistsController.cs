using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMusicApp.Data;
using MyMusicApp.Models;

namespace MyMusicApp.Controllers;

[Authorize]
public class PlaylistsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PlaylistsController> _logger;

    public PlaylistsController(ApplicationDbContext context, ILogger<PlaylistsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: /Playlists
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userPlaylists = await _context.Playlists
            .Where(p => p.UserId == userId)
            .Select(p => new Playlist { Id = p.Id, Name = p.Name })
            .ToListAsync();
        return View(userPlaylists);
    }

    // GET: /Playlists/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var playlist = await _context.Playlists
            .Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (playlist == null) return NotFound();
        return View(playlist);
    }

    // GET: /Playlists/Create
    public IActionResult Create()
    {
        return View(new PlaylistCreateViewModel());
    }

    // POST: /Playlists/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlaylistCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var playlist = new Playlist
            {
                Name = viewModel.Name,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            _context.Add(playlist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    // --- THÊM CÁC ACTION MỚI ĐỂ XÓA PLAYLIST ---
    // GET: Playlists/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var playlist = await _context.Playlists
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (playlist == null) return NotFound();
        return View(playlist);
    }

    // POST: Playlists/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (playlist != null)
        {
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // --- THÊM API MỚI ĐỂ XÓA BÀI HÁT KHỎI PLAYLIST ---
    [HttpPost]
    public async Task<IActionResult> RemoveSongFromPlaylist(int songId, int playlistId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var playlist = await _context.Playlists.AsNoTracking().FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId);
        if (playlist == null)
        {
            return Json(new { success = false, message = "Không tìm thấy playlist." });
        }

        var playlistSong = await _context.PlaylistSongs
            .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

        if (playlistSong != null)
        {
            _context.PlaylistSongs.Remove(playlistSong);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã xóa bài hát khỏi playlist." });
        }

        return Json(new { success = false, message = "Bài hát không có trong playlist." });
    }

    // GET API: Lấy danh sách playlist của người dùng
    [HttpGet]
    public async Task<IActionResult> GetUserPlaylists()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var playlists = await _context.Playlists
            .Where(p => p.UserId == userId)
            .Select(p => new { p.Id, p.Name })
            .ToListAsync();
        return Json(playlists);
    }

    // POST API: Thêm một bài hát vào playlist
    [HttpPost]
    public async Task<IActionResult> AddSongToPlaylist(int songId, int playlistId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId);
        if (playlist == null) return Json(new { success = false, message = "Playlist không hợp lệ." });
        var songExists = await _context.Songs.AnyAsync(s => s.Id == songId);
        if (!songExists) return Json(new { success = false, message = "Bài hát không tồn tại." });
        var songAlreadyInPlaylist = await _context.PlaylistSongs.AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
        if (songAlreadyInPlaylist) return Json(new { success = false, message = "Bài hát đã có trong playlist." });
        var playlistSong = new PlaylistSong { PlaylistId = playlistId, SongId = songId };
        _context.PlaylistSongs.Add(playlistSong);
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Đã thêm bài hát vào playlist." });
    }
}
