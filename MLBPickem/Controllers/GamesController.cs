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
                .ThenBy(g => g.HomeTeam.TeamId)
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
                .OrderByDescending(ug => ug.Game.FirstPitchDateTime.DayOfYear)
                .ThenBy(ug => ug.Game.FirstPitchDateTime.TimeOfDay)
                .ThenBy(ug => ug.Game.HomeTeam.TeamId)
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
        public async Task<IActionResult> UpdateUserGames([FromForm] int IsChecked, int TeamId, int GameId, DateTime FirstPitch, string scrollPos)
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
                    if (oldUserGame != null)
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

                    // ***** Since this method can be accessed from Games/Index AND Games/MyPicks,
                    // this returns the user to whichever view sent them to this method.
                    // The scroll position is sent back to the view as TempData to be used to set the view to where the user
                    // was when the UpdateUserGames method was tripped******
                    double SP = double.Parse(scrollPos);
                    TempData["ScrollPos"] = SP;
                    string referer = Request.Headers["Referer"].ToString();
                    return Redirect(referer);
                }
                // This scenario means a user is unchecking a previously chosen team
                else
                {
                    UserGame oldUserGame = (UserGame)_context.UserGames
                        .Where(ug => ug.GameId == GameId)
                        .Where(ug => ug.UserId == user.Id)
                        .FirstOrDefault()
                        ;

                    _context.Remove(oldUserGame);
                    await _context.SaveChangesAsync();

                    //explained in previous return
                    double SP = double.Parse(scrollPos);
                    TempData["ScrollPos"] = SP;
                    string referer = Request.Headers["Referer"].ToString();
                    return Redirect(referer);
                }
            }
            // User is attempting to make a choice after the game has started
            else
            {
                // explained in previous return
                double SP = double.Parse(scrollPos);
                TempData["ScrollPos"] = SP;
                string referer = Request.Headers["Referer"].ToString();
                return Redirect(referer);
            }
        }

        public async Task<IActionResult> Standings()
        {
            // Get all user games for complete games, grouped by user
            var completedGamesByUser = _context.UserGames
                .Include(ug => ug.Game)
                .ThenInclude(g => g.AwayTeam)
                .Include(ug => ug.Game)
                .ThenInclude(g => g.HomeTeam)
                .Where(ug => ug.Game.GameComplete == true)
                .GroupBy(ug => ug.User)
                .ToList();
                ;

            //Make a list of UserScore objects to send to the view
            List<UserScore> UserScores = new List<UserScore>();

            //Calculate total score for User
            foreach (var userGroup in completedGamesByUser)
            {
                // set total score initially to 0
                var userScore = 0;
                foreach (var userGame in userGroup)
                {
                    // IF USER CHOSE WINNING TEAM
                    if (userGame.ChosenTeamId == userGame.Game.WinningTeamId)
                    {
                        // IF USER CHOSE AWAY TEAM
                        if (userGame.ChosenTeamId == userGame.Game.AwayTeam.TeamId)
                        {
                            //IF USER CHOSE UNDERDOG
                            if (userGame.Game.AwayLine.StartsWith("+"))
                            {
                                userScore = userScore + Int32.Parse(userGame.Game.AwayLine);
                            }
                            //IF USER CHOSE FAVORITE
                            else
                            {
                                userScore = userScore + 100;
                            }
                        }
                        // IF USER CHOSE HOME TEAM
                        else
                        {
                            //IF USER CHOSE UNDERDOG
                            if (userGame.Game.HomeLine.StartsWith("+"))
                            {
                                userScore = userScore + Int32.Parse(userGame.Game.HomeLine);
                            }
                            //IF USER CHOSE FAVORITE
                            else
                            {
                                userScore = userScore + 100;
                            }

                        }
                    }
                    // IF USER CHOSE LOSING TEAM
                    else
                    {
                        // IF USER CHOSE AWAY TEAM
                        if (userGame.ChosenTeamId == userGame.Game.AwayTeam.TeamId)
                        {
                            //IF USER CHOSE UNDERDOG
                            if (userGame.Game.AwayLine.StartsWith("+"))
                            {
                                userScore = userScore - 100;
                            }
                            //IF USER CHOSE FAVORITE
                            else
                            {
                                userScore = userScore - Int32.Parse(userGame.Game.AwayLine) * (-1);
                            }
                        }
                        // IF USER CHOSE HOME TEAM
                        else
                        {
                            //IF USER CHOSE UNDERDOG
                            if (userGame.Game.HomeLine.StartsWith("+"))
                            {
                                userScore = userScore - 100;
                            }
                            //IF USER CHOSE FAVORITE
                            else
                            {
                                userScore = userScore - Int32.Parse(userGame.Game.HomeLine) * (-1);
                            }

                        }
                    }
                }

                UserScore userScoreObj = new UserScore
                {
                    User = userGroup.Key,
                    TotalScore = userScore
                };

                UserScores.Add(userScoreObj);
            }

            UserScores.OrderBy(us => us.TotalScore);

            return View(UserScores);
        }
    }
}