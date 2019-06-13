using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLBPickem.Models.ViewModels
{
    public class AvailableGame
    {
        public Game Game { get; set; }
        public int GameId { get; set; }
        public UserGame UserGame { get; set; }
        public int UserGameId { get; set; }
        public int TeamId { get; set; }
        public int IsChecked { get; set; }
    }
}
