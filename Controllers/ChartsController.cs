using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMusicApp.Data;

namespace MyMusicApp.Controllers
{
    public class ChartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Charts
        public async Task<IActionResult> Index()
        {
            // Lấy 100 bài hát có lượt nghe cao nhất, sắp xếp giảm dần
            var top100Songs = await _context.Songs
                .OrderByDescending(s => s.ListenCount)
                .Take(100)
                .ToListAsync();

            return View(top100Songs);
        }
    }
}
