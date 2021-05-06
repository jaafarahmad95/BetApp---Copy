using System;
using System.Collections.Generic;
using System.Text;
using testtest.Models.Site;

namespace testtest.dto.Site
{
    public class ScoreBoardDTO
    {
        public int  redCards1 { get; set; }
        public int redCards2 { get; set; }
        public int penalties1 { get; set; }
        public int penalties2 { get; set; }
        public int yellowCards1 { get; set; }
        public int yellowCards2 { get; set; }
        public IEnumerable<Messages> messages { get; set; }
       
        public string score { get; set; }

        public string teamName1 { get; set; }
        public string teamName2 { get; set; }
        public string shirtColor1 { get; set; }
       
        public string shortsColor1 { get; set; }

        public string shirtColor2 { get; set; }

        public string shortsColor2 { get; set; }

        public int firsthalf1teamscore { get; set; }
        public int firsthalf2teamscore { get; set; }

        public int seconedhalf1teamscore { get; set; }
        public int seconedhalf2teamscore { get; set; }

        public int firstEhalf1teamscore { get; set; }
        public int seconedEhalf1teamscore { get; set; }
        public int firstEhalf2teamscore { get; set; }
        public int seconedEhalf2teamscore { get; set; }


        public string period { get; set; }
    }
}
