using Microsoft.AspNetCore.Mvc;
using MyMusicApp.Data;

namespace MyMusicApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SongsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api/SongsApi/IncrementListenCount/5
        [HttpPost("IncrementListenCount/{id}")]
        public async Task<IActionResult> IncrementListenCount(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            song.ListenCount++;
            await _context.SaveChangesAsync();

            return Ok(new { newListenCount = song.ListenCount });
        }
    }
}
