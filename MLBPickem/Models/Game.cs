using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MLBPickem.Models
{
    public class Game
    {

        [Key]
        public int GameId { get; set; }

        [Required]
        public DateTime FirstPitchDateTime { get; set; }

        [Required]
        public int AwayTeamId { get; set; }

        [Required]
        public string AwayLine { get; set; }

        public string AwayStartingPitcher { get; set; }

        [Required]
        public int HomeTeamId { get; set; }

        [Required]
        public string HomeLine { get; set; }

        public string HomeStartingPitcher { get; set; }

        public int AwayScore { get; set; }

        public int HomeScore { get; set; }

        public bool GameStarted { get; set; }

        public bool GameComplete { get; set; }

        public string Inning { get; set; }

        public int WinningTeamId { get; set; }

        public int MLBScoreBoardId { get; set; }
    }
}
