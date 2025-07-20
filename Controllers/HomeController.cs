using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMusicApp.Data;
using MyMusicApp.Models;
using System.Diagnostics;

namespace MyMusicApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel
        {
            
            LatestSongs = await _context.Songs
                                .OrderByDescending(s => s.Id)
                                .Take(5)
                                .ToListAsync(),

           
            ChartSongs = await _context.Songs
                                .OrderByDescending(s => s.ListenCount)
                                .Take(10)
                                .ToListAsync(),

           
            FeaturedPlaylists = await _context.Songs
                                        .Take(5)
                                        .ToListAsync()
        };

        return View(viewModel);
    }

  
    public async Task<IActionResult> Search(string query)
    {
        ViewData["CurrentQuery"] = query;

        if (string.IsNullOrEmpty(query))
        {
            return View(new List<Song>());
        }

    
        var searchResults = await _context.Songs
            .Where(s => EF.Functions.Like(s.Title, $"%{query}%") || EF.Functions.Like(s.Artist, $"%{query}%"))
            .ToListAsync();

        return View(searchResults);
    }
    // GET: /Home/Contact
    public IActionResult Contact()
    {
        return View();
    }

    // POST: /Home/Contact
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(Feedback model)
    {
        if (ModelState.IsValid)
        {
            _context.Feedbacks.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi phản hồi! Chúng tôi sẽ xem xét sớm nhất có thể.";
            return RedirectToAction("Index");
        }

   
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in errors)
        {
            _logger.LogError("ModelState Error on Contact form: {ErrorMessage}", error.ErrorMessage);
            if (error.Exception != null)
            {
                _logger.LogError("Exception (if any): {Exception}", error.Exception.Message);
            }
        }
  

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
