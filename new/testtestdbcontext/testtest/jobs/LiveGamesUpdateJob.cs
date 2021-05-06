using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using testtest.dto.Site;
using testtest.Service.Site;
using testtest.SignalRHub;

namespace testtest.jobs
{
    public class LiveGamesUpdateJob
    {

            
        private readonly IEventServices _event ;
        private readonly IMapper _mapper;
         private readonly IHubContext<LiveGameHub> _LHub;
        public LiveGamesUpdateJob(IEventServices eve,IMapper mapper
        ,IHubContext<LiveGameHub> LHub)
        {
            _event = eve;
            _mapper=mapper;
            _LHub= LHub;
        }

         public async Task Updatelivegames()
        {
            var R = await _event.GetLiveEvent();
         
            var c = _mapper.Map<IEnumerable<EventViewDto>>(R);
            
            await PushToSignalR(c);
        }
        private async Task<bool> PushToSignalR(IEnumerable<EventViewDto> updates)
        {
            
            await _LHub.Clients.Group("LiveGameGroup").SendAsync("LiveMatchesUpdate", updates);
            return true;
        }
    }
}