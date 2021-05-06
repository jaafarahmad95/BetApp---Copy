using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto.Site
{
    public class CancelUnmatchedDto
    {
        public string userid { get; set; }
        public double odds { get; set; }
        public string eventid { get; set; }
        public string MarketName { get; set; }
        public string Side { get; set; }
        public string RunnerName { get; set; }
    }
}
