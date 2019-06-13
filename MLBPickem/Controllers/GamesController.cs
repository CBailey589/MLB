using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLBPickem.Data;
using MLBPickem.Models;

namespace MLBPickem.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GamesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);



        //GET: Games/Index
        public async Task<IActionResult> Index()
        {
            // Get Current Time in Eastern Time
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            // Get today's games that have not started
            var TodaysUpcomingGames = _context.Games
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .Where(g => g.FirstPitchDateTime > easternTime)
                .OrderBy(g => g.FirstPitchDateTime)
                .ToList()
                ;
            return View(TodaysUpcomingGames);
        }
    }
}