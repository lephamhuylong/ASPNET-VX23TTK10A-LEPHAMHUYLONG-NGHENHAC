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

     
        public async Task<IActionResult> Index()
        {
        
            var top100Songs = await _context.Songs
                .OrderByDescending(s => s.ListenCount)
                .Take(100)
                .ToListAsync();

            return View(top100Songs);
        }
    }
}
