using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using testtest.dto.Site;
using testtest.Service.Site;
using testtest.SignalRHub;

namespace testtest.jobs
{
    public class BetsSetJob 
    {
        
        private readonly IBetServices _BetS ;

        public BetsSetJob(IBetServices BetS  )
        {
            _BetS=BetS;
        }

        public async Task SetBets()
        {
            await _BetS.SattelBets();
        }

       

    }
}
