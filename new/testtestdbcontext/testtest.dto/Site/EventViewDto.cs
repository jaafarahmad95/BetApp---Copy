using System;
using System.Collections.Generic;
using System.Text;
using testtest.Models.Site;

namespace testtest.dto.Site
{
    public class EventViewDto
    {
        public int EventId { get; set; }

        public string HomeTeam { get; set; }
        
        public string AwayTeam { get; set; }

        public DateTime startDate { get; set; }

        public string Score { get; set; }
        public ScoreBoared scoreboard { get; set; }
    }
}
