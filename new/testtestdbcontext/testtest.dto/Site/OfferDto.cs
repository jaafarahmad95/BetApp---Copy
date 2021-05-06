using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto.Site
{
    public class OfferDto
    {
        public int EventId { get; set; }
        public string Teams { get; set; }
        public string UserId { get; set; }

        public string MarketName { get; set; }
        public string Side { get; set; } 
        
        public string RunnerName { get; set; }
    
        public double Odds { get; set; }
      
        public bool KeepInPlay { get; set; }
     
        public string Status { get; set; }

        public double Stake { get; set; }

    }
}
