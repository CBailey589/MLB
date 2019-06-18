using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLBPickem.Models.ViewModels
{
    public class UserScore
    {
        public ApplicationUser User { get; set; }
        public int TotalScore { get; set; }
    }
}
