using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Quartz;
using testtest.dto.Site;
using testtest.Service.Site;
using testtest.SignalRHub;
using testtestdbcontext.testtest.SignalRHub;

namespace testtest.jobs
{
    public class UpdateScoreBoard : IJob
    {

        
        private readonly IHubContext<ScoreBoardHub> _SHub;
        private readonly IEventServices _evser;
         private readonly IMapper _mapper;
        public UpdateScoreBoard(/*IHubContext<BetSystemHub> hub,*/ IEventServices evser,
        IHubContext<ScoreBoardHub> SHub,IMapper mapper)
        {
            
            _evser = evser;
            _SHub=SHub;
            _mapper=mapper;
        } 
        public async Task Execute(IJobExecutionContext context)
        {    
             JobDataMap data = context.JobDetail.JobDataMap;
             int EventId = data.GetInt("EventId");
             var R = await _evser.GetScoreBoared(EventId);
             var c = _mapper.Map<ScoreBoardDTO>(R);
             await PushToSignalR(EventId,c);
             
        }
        private async Task<bool> PushToSignalR(int eventId,ScoreBoardDTO updates)
        {
            var y = $"event-{eventId}-update";
            await _SHub.Clients.Group($"event-{eventId}-update").SendAsync("ScoreBoardUpdate", updates);
            return true;
        }
    }
}