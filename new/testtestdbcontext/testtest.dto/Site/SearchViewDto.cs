using System;

namespace testtestdbcontext.testtest.dto.Site
{
    public class SearchViewDto
    {
          public int EventId { get; set; }

        public string HomeTeam { get; set; }
        
        public string AwayTeam { get; set; }

        public DateTime startDate { get; set; }

        public string Score { get; set; }
        public string Stage { get; set; }
    }
}