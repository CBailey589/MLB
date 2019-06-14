using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLBPickem.Data;
using MLBPickem.Models;
using MLBPickem.Models.ViewModels;

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


        [Authorize]
        //GET: Games/Index
        public async Task<IActionResult> Index()
        {
            // Get Current Time in Eastern Time
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            // Get Current User
            var user = await GetCurrentUserAsync();

            // Get today's games that have not started
            var TodaysUpcomingGames = _context.Games
                .Include(g => g.AwayTeam)
                .Include(g => g.HomeTeam)
                .Include(g => g.UserGames)
                .Where(g => g.FirstPitchDateTime > easternTime)
                .OrderBy(g => g.FirstPitchDateTime)
                .ToList()
                ;

            // Make list to hold Available Game models
            List<AvailableGame> AvailableGameModels = new List<AvailableGame>();

            // Convet list of games into list of AvailableGames to send to model
            TodaysUpcomingGames.ForEach(game =>
            {
                //Check to see if there is a usergame associated with this user and game
                var userGame = game.UserGames.Where(ug => ug.User.Id == user.Id).FirstOrDefault();
                // set defaults to 0 assuming there is no UserGame for this user/game
                var isChecked = 0;
                int userGameId = 0;
                int teamId = 0;
                if (userGame != null)
                {
                    // change to true if a UserGame exists for this game and user
                    isChecked = 1;
                    userGameId = userGame.UserGameId;
                    teamId = userGame.ChosenTeamId;
                }
                // Crate AvailableGame for each game
                AvailableGame newAvailableGameModel = new AvailableGame()
                {
                    Game = game,
                    GameId = game.GameId,
                    UserGame = userGame,
                    UserGameId = userGameId,
                    TeamId = teamId,
                    IsChecked = isChecked
                };

                AvailableGameModels.Add(newAvailableGameModel);
            });

            return View(AvailableGameModels);
        }

        [Authorize]
        //GET: Games/MyPicks
        public async Task<IActionResult> MyPicks()
        {
            // Get Yesterday
            var yesterday = DateTime.Today.AddDays(-1);

            // Get Current User
            var user = await GetCurrentUserAsync();

            // Get games user picked yesterday and today
            var UsersPickedGames = _context.UserGames
                .Where(ug => ug.UserId == user.Id)
                .Include(ug => ug.User)
                .Include(ug => ug.Game).ThenInclude(g => g.AwayTeam)
                .Include(ug => ug.Game).ThenInclude(g => g.HomeTeam)                
                .Where(ug => ug.Game.FirstPitchDateTime > yesterday)
                .OrderBy(ug => ug.Game.FirstPitchDateTime)
                .ToList();

            // Make list to hold Available Game models
            List<AvailableGame> AvailableGameModels = new List<AvailableGame>();

            // Convet list of usergames into list of AvailableGames to send to model
            UsersPickedGames.ForEach(usergame =>
            {
                // Crate AvailableGame for each game
                AvailableGame newAvailableGameModel = new AvailableGame()
                {
                    Game = usergame.Game,
                    GameId = usergame.Game.GameId,
                    UserGame = usergame,
                    UserGameId = usergame.UserGameId,
                    TeamId = usergame.ChosenTeamId,
                    IsChecked = 1
                };

                AvailableGameModels.Add(newAvailableGameModel);
            });

            return View(AvailableGameModels);
        }

        [Authorize]
        public async Task<IActionResult> UpdateUserGames([FromForm] int IsChecked, int TeamId, int GameId, DateTime FirstPitch)
        {
            // Get Current Time in Eastern Time
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            // Get Current User
            var user = await GetCurrentUserAsync();

            //Make sure easternTime isn't later than FirstPitchDateTime
            if (FirstPitch > easternTime)
            {
                // This scenario means either the user had previously chosen 1 team, and is now chosing the other,
                // OR that the user is chosing a team for the first time
                if (IsChecked == 1)
                {
                    //Check to see if there is a UserGame alrady in the DB
                    UserGame oldUserGame = (UserGame)_context.UserGames
                        .Where(ug => ug.GameId == GameId)
                        .Where(uh => uh.UserId == user.Id)
                        .FirstOrDefault()
                        ;

                    //If oldUserGame exists, remove from DB
                    if(oldUserGame != null)
                    {
                        _context.Remove(oldUserGame);
                    }

                    //Make new UserGame for DB
                    UserGame newUserGame = new UserGame()
                    {
                        GameId = GameId,
                        UserId = user.Id,
                        ChosenTeamId = TeamId
                    };
                    _context.UserGames.Add(newUserGame);
                    await _context.SaveChangesAsync();

                    // Redirect back to Games/Index
                    return RedirectToAction("Index", "Games");
                }
                // This scenario means a user is unchecking a previously chosen team
                else
                {
                    UserGame oldUserGame = (UserGame)_context.UserGames
                        .Where(ug => ug.GameId == GameId)
                        .Where(uh => uh.UserId == user.Id)
                        .FirstOrDefault()
                        ;

                    _context.Remove(oldUserGame);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Games");
                }
            }
            // User is attempting to make a choice after the game has started
            else
            {
                return RedirectToAction("Index", "Games");
            }
        }
    }
}