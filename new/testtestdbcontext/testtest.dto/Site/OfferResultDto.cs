using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto.Site
{
    public class OfferResultDto
    {
        public string RunnerName { get; set; }
        public string MarketName { get; set; }
        public int EventId { get; set; }
        public string Side { get; set; }
        public double Odds { get; set; }
        
        public bool KeepInPlay { get; set; }

       
        public string Status { get; set; }
        
        public double Liquidity { get; set; }

        public string visibility { get; set; }
    }
}
