using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMusicApp.Areas.Admin.Models;
using MyMusicApp.Data;
using MyMusicApp.Models;

namespace MyMusicApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SongsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var songs = await _context.Songs.Include(s => s.Genre).ToListAsync();
            return View(songs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var song = await _context.Songs.Include(s => s.Genre).FirstOrDefaultAsync(m => m.Id == id);
            if (song == null) return NotFound();
            return View(song);
        }

        public IActionResult Create()
        {
            var viewModel = new SongViewModel { Genres = _context.Genres.ToList() };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string? musicFilePath = await UploadFile(viewModel.MusicFile, "audio");
                string? imageFilePath = await UploadFile(viewModel.ImageFile, "images/covers");

                if (musicFilePath == null)
                {
                    ModelState.AddModelError("MusicFile", "Lỗi trong quá trình tải lên file nhạc.");
                    viewModel.Genres = _context.Genres.ToList();
                    return View(viewModel);
                }

                var song = new Song
                {
                    Title = viewModel.Title,
                    Artist = viewModel.Artist,
                    GenreId = viewModel.GenreId,
                    FilePath = musicFilePath,
                    ImageUrl = imageFilePath
                };

                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            viewModel.Genres = _context.Genres.ToList();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var song = await _context.Songs.FindAsync(id);
            if (song == null) return NotFound();
            ViewData["GenreId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Artist,FilePath,ImageUrl,ListenCount,DownloadCount,GenreId")] Song song)
        {
            if (id != song.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var song = await _context.Songs.Include(s => s.Genre).FirstOrDefaultAsync(m => m.Id == id);
            if (song == null) return NotFound();
            return View(song);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
                DeletePhysicalFile(song.FilePath);
                DeletePhysicalFile(song.ImageUrl);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }

        private async Task<string?> UploadFile(IFormFile? file, string subfolder)
        {
            if (file == null || file.Length == 0) return null;
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, subfolder);
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/{subfolder.Replace('\\', '/')}/{uniqueFileName}";
        }

        private void DeletePhysicalFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}

