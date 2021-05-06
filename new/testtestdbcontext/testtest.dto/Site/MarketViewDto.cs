using System;
using System.Collections.Generic;
using System.Text;


namespace testtest.dto.Site
{
   public class MarketViewDto
    {
        public double Odds { get; set; }
        
        public string Name { get; set; }
        

        public string Visibility { get; set; }
        
        public int Numerator { get; set; }
        
        public int Denominator { get; set; }
       
        public int AmericanOdds { get; set; }
    }
}
