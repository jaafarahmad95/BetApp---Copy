using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using testtest.dto;
using testtest.dto.Site;
using testtest.jobs;
using testtest.Models.Site;
using testtest.Service.Site;
using testtestdbcontext.testtest.dto.Site;

namespace testtest.Controllers
{
    [Produces("application/json")]
    public class EventController : ControllerBase
    {
        private readonly IEventServices _eventService;
        private readonly IMapper _mapper;
        IScheduler _scheduler2;
        public EventController(IEventServices eventServices,IMapper mapper,IScheduler scheduler2)
        {
            _eventService = eventServices;
            _mapper = mapper;
            _scheduler2 = scheduler2;
        }

        [HttpGet(ApiRoutes.Events.GetRegion, Name = "GetRegion")]
        public async Task<IActionResult> GetRegion()
        {
            var R = await _eventService.GetRegion();
            
            return Ok(R);
        }

        [HttpGet(ApiRoutes.Events.GetCompInRegion, Name = "GetCompRegion")]
        public async Task<ActionResult> GetCompInRegion(string region)
        {
            var R = await _eventService.GetCompInRegion(region);

            return Ok(R);
        }

        [HttpGet(ApiRoutes.Events.GetEventincomp, Name = "GetEventincomp")]
        public async Task<ActionResult> GetEventInComp(string region, string comp)
        {
            var R = await _eventService.GetEventInComp(region,comp);
            var c = _mapper.Map<IEnumerable<EventViewDto>>(R);
            return Ok(c);
        }

        [HttpGet(ApiRoutes.Events.GetLiveEvents, Name = "GetLiveEvents")]
        public async Task<ActionResult> GetLiveEvents()
        {
            var R = await _eventService.GetLiveEvent();
         
            var c = _mapper.Map<IEnumerable<EventViewDto>>(R);
           return Ok(c);
        }

        [HttpGet(ApiRoutes.Events.GetEventmarkets, Name = "GetEventmarkets")]
        public async Task<ActionResult> GetEventmarkets(int eventId)
        {
            var R = await _eventService.GetEventMarkets(eventId);
          
            return Ok(R) ;
        }

        [HttpGet(ApiRoutes.Events.GetScoreBoared, Name = "GetScoreBoared")]
        public async Task<ActionResult> GetScoreBoared(string userId,int eventId)
        {
            
             var trigforusr="foruser"+userId+"ScoreB";
             var grpnameuser = "quartzUpdateSBTriggers"+userId ;
             var names = _scheduler2.GetTriggerGroupNames().Result;
             foreach(var item in names)
             {
                    if (item.ToString()==grpnameuser)
                        await _scheduler2.UnscheduleJob(new TriggerKey(trigforusr,grpnameuser));
                        
             }
            var jobid =   "foruser"+userId;      
            var jobgroup = "quartzUpdateSB";
            JobKey jk = new Quartz.JobKey (jobid, jobgroup);
            await _scheduler2.DeleteJob(jk);           
             
            

            IJobDetail job = JobBuilder.Create<UpdateScoreBoard>()
                                       .UsingJobData("EventId", eventId)
                                       .WithIdentity(/*"UpdateSBJob"+eventId+*/"foruser"+userId, "quartzUpdateSB")
                                       .RequestRecovery()
                                       .StoreDurably(false)
                                       .Build();
            ITrigger trigger = TriggerBuilder.Create()
                                             .ForJob(job)
                                             .UsingJobData("EventId", eventId)
                                             .WithIdentity("foruser"+userId+"ScoreB", "quartzUpdateSBTriggers"+userId)
                                             .StartNow()
                                             .WithSimpleSchedule(z => z.WithIntervalInSeconds(1).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
                                             .Build();

            await _scheduler2.ScheduleJob(job,trigger);
            var jobsinpro = _scheduler2.GetCurrentlyExecutingJobs();
            

            return Ok();
        }
        [HttpGet(ApiRoutes.Events.GetRegAndComp,Name ="GetRegionsAndComps")]
        public async Task<IActionResult> GetRegionsAndComps()
        {
            var r = await _eventService.GetRegComp();
            return Ok(r);
        }

        [HttpGet(ApiRoutes.Events.search,Name ="search")]
        public async Task<IActionResult> search(string SearchTearm)
        {
            var res = await _eventService.Search(SearchTearm);
            var c = _mapper.Map<IEnumerable<SearchViewDto>>(res);
            return Ok(c);
        }


    }
}
