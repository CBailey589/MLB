using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MLBPickem.Models
{
    public class UserGame
    {
        [Key]
        public int UserGameId { get; set; }

        [Required]
        public int GameId { get; set; }

        public Game Game { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public int ChosenTeamId { get; set; }
    }
}
