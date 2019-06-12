using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MLBPickem.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string VIName { get; set; }

        [Required]
        public string MLBName { get; set; }
    }
}
