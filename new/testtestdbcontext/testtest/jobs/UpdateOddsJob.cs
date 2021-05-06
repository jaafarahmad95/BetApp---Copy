using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using testtest.Service.Site;
using testtest.SignalRHub;

namespace testtest.jobs
{
    public class UpdateOddsJob : IJob
    {
        

        private readonly IHubContext<BetSystemHub> _hub;
        private readonly IHubContext<LiveGameHub> _LHub;
        private readonly IOfferService _OfferService;
        public UpdateOddsJob(IHubContext<BetSystemHub> hub, IOfferService offerService,IHubContext<LiveGameHub> LHub)
        {
            _hub = hub;
            _OfferService = offerService;
            _LHub=LHub;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap data = context.JobDetail.JobDataMap;
            int EventId = data.GetInt("EventId");
            string MarketName = data.GetString("MarketName");
            var one = await _OfferService.Getoffers(EventId, MarketName);
            await PushToSignalR(one);
           
        }
        private async Task<bool> PushToSignalR(List<dto.Site.OfferResultDto> updates)
        {
            var x = updates[0].EventId;
            var z = updates[0].MarketName;
            var y = $"event-{x}-update";
            await _hub.Clients.Group($"event-{x}-update{z}").SendAsync("liveGameUpdate", updates);
            await _LHub.Clients.Group($"event-{x}-update{z}").SendAsync("liveOddUpdate", updates);
            return true;
        }
    }
}
