using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using testtest.dto;
using testtest.dto.Site;
using testtest.Models.Site;
using testtest.Service;
using testtest.Service.Site;
using testtest.Service.Extensions;
using NotificationSystem.Domain;
using testtest.SignalRHub;
using Microsoft.AspNetCore.SignalR;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using testtest.jobs;
using Quartz;
using testtestdbcontext.AppUserService.Site;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using testtest.Models.SignalRConnection;

namespace testtest.Controllers
{
    [Produces("Application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   
    public class OfferController: ControllerBase
    {
        IScheduler _scheduler;
      
        private readonly IOfferService _OfferService;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IEventServices _eventService;
        private readonly IHubContext<BetSystemHub> _BetSystemHub;
        private readonly IHubContext<LiveGameHub> _LiveGameHub;

        private readonly IBetCounterService _BetCSer ;
        public IServiceProvider _ServiceProvider;
         private readonly IConnectionService _conser;

        public OfferController(IOfferService offerservice,IMapper mapper , 
            IAccountService accountService, IEventServices eventService,
            IHubContext<BetSystemHub> hub,
            IHubContext<LiveGameHub> Lhub,
            IServiceProvider serviceProvider,
            IScheduler scheduler,IConnectionService conser,
            IBetCounterService BetCSer)
        {
            _OfferService = offerservice;
            _mapper = mapper;
            _accountService = accountService;
            _eventService = eventService;
            _BetSystemHub = hub;
            _ServiceProvider = serviceProvider;
            _scheduler = scheduler;
            _BetCSer=BetCSer;
         
            _conser=conser;
            _LiveGameHub = Lhub;
        }

        
        [HttpGet(ApiRoutes.Offers.getoffers,Name="getOffers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOffers(string userId,int EventId,string MarketName)
        {
             
             var trigforusr="foruser"+userId;
             var grpnameuser = "quartzUpdateOddsTriggers"+userId ;
             var names = _scheduler.GetTriggerGroupNames().Result;
             foreach(var item in names)
             {
                    if (item.ToString()==grpnameuser)
                        await _scheduler.UnscheduleJob(new TriggerKey(trigforusr,grpnameuser));
                        
             }
             
            var jobid =   "foruser"+userId;      
            var jobgroup = "quartzUpdateOdds";
            JobKey jk = new Quartz.JobKey (jobid, jobgroup);
            await _scheduler.DeleteJob(jk);
            

            IJobDetail job = JobBuilder.Create<UpdateOddsJob>()
                                       .UsingJobData("EventId", EventId)
                                       .UsingJobData("MarketName", MarketName)
                                       .WithIdentity(/*"UpdateOddsJob"+EventId+"and market"+MarketName+*/"foruser"+userId, "quartzUpdateOdds")
                                       .RequestRecovery()
                                       .StoreDurably(false)
                                       .Build();
            ITrigger trigger = TriggerBuilder.Create()
                                             .ForJob(job)
                                             .UsingJobData("EventId", EventId)
                                             .UsingJobData("MarketName", MarketName)
                                             .WithIdentity("foruser"+userId, "quartzUpdateOddsTriggers"+userId)
                                             
                                             .StartNow()
                                             .WithSimpleSchedule(z => z.WithIntervalInSeconds(1).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
                                             .Build();

            await _scheduler.ScheduleJob(job,trigger);

           // var jobsinpro = _scheduler.GetCurrentlyExecutingJobs();
            return Ok();
        }

      
        [HttpPost(ApiRoutes.Offers.PlaceLaybet, Name = "placelaybet")]
        [Authorize(Roles ="Client")]
        public async Task<IActionResult> PlaceLayOffer([FromBody] List<OfferDto> offer)
        {
            double laibelty = 0.00;
            foreach (var item in offer)
            {
                laibelty = laibelty + item.Stake*item.Odds;
            }
            if (await _OfferService.CheckLaiaabilty(laibelty, offer[0].UserId) == false)
                return BadRequest("you don't have enough cash to place this bet");
            else await _accountService.TakeFromBalance(offer[0].UserId, laibelty);
            List<List<Bet>> bettoreturn = new List<List<Bet>>();
            foreach (var item in offer)
            {
                item.MarketName = item.MarketName.Replace("%2F", "/");
               
               var Backexsist = await _OfferService.CheckExsitingBackbets(item.Odds, item.MarketName, item.EventId, item.RunnerName);
                if (Backexsist != null)
                {
                    var exsist = await _OfferService.GetBackOffer(Backexsist);
                    if (exsist.Liquidity != 0)
                    {
                        if (exsist.Liquidity == item.Stake || exsist.Liquidity > item.Stake)
                        {

                            await _OfferService.TakeFromBackLiquidity(exsist.Id, item.Stake);
                            var matchedbet = _mapper.Map<Bet>(item);
                            var betinvolved = await _OfferService.ManageInvolvedBackBets(exsist.Id, matchedbet);
                            matchedbet.Status = "closed";
                            matchedbet.RemainingStake = 0;
                            matchedbet.InStake = item.Stake;
                            matchedbet.Lalaibelty = laibelty;
                            matchedbet.Profit = item.Stake;
                            var bettoadd = await _OfferService.AddToBets(matchedbet);
                            await _BetCSer.addtoCounter(matchedbet.EventId);
                            await _OfferService.AddToBackMatchedbet(exsist.Id, bettoadd);
                           
                            betinvolved.Add(bettoadd);
                            SendNotification(betinvolved, "MatchingBet");
                           
                            bettoreturn.Add(betinvolved);
                        }
                        else if (exsist.Liquidity < item.Stake)
                        {
                            var remainingStake = item.Stake - exsist.Liquidity;
                            var matchedbet = _mapper.Map<Bet>(item);
                            var betinvolved = await _OfferService.ManageInvolvedLowBackBets(exsist.Id, matchedbet);
                            matchedbet.InStake = exsist.Liquidity;
                            matchedbet.RemainingStake = 0;
                            matchedbet.Status = "closed";
                            matchedbet.Lalaibelty = exsist.Liquidity * item.Odds;
                            matchedbet.Profit = exsist.Liquidity;
                            var bettoadd = await _OfferService.AddToBets(matchedbet);
                             await _BetCSer.addtoCounter(matchedbet.EventId);
                            await _OfferService.AddToBackMatchedbet(exsist.Id, bettoadd);
                            var x = _mapper.Map<LayOffer>(item);
                            x.Liquidity = remainingStake;
                            var newlayack = await _OfferService.AddLayOffer(x);
                            var c = _mapper.Map<Bet>(x);
                            var added = await _OfferService.AddToBets(c);
                             await _BetCSer.addtoCounter(added.EventId);
                            var betLayinvoled = _mapper.Map<InvolvedLayBets>(added);
                            await _OfferService.AddToInvolvedLayBets(newlayack.Id, betLayinvoled);
                         
                            betinvolved.Add(bettoadd);
                           SendNotification(betinvolved, "MatchingBet");
                            bettoreturn.Add(betinvolved);
                        }
                    }
                }
                var Layexsist = await _OfferService.CheckExsitingLaybets(item.Odds, item.MarketName, item.EventId, item.RunnerName);
                if (Layexsist != null)
                {
                    List<Bet> laybets = new List<Bet>();
                    await _OfferService.AddToLayLiquidity(Layexsist, item.Stake);
                    var laykbet = _mapper.Map<Bet>(item);
                    laybets.Add(laykbet);
                    var added = await _OfferService.AddToBets(laykbet);
                     await _BetCSer.addtoCounter(added.EventId);
                    var involved = _mapper.Map<InvolvedLayBets>(added);
                    involved.Status = "open";
                    await _OfferService.AddToInvolvedLayBets(Layexsist, involved);
                    SendNotification(laybets, "MatchingBet");
                    bettoreturn.Add(laybets);
                   
                }
                if(Layexsist==null && Backexsist==null)
                {
                    List<Bet> newlaybet = new List<Bet>();
                    var betoffer = _mapper.Map<LayOffer>(item);
                    var Layoffer = await _OfferService.AddLayOffer(betoffer);
                    var bet = _mapper.Map<Bet>(betoffer);
                    newlaybet.Add(bet);
                    var added2 = await _OfferService.AddToBets(bet);
                    await _BetCSer.addtoCounter(added2.EventId);
                    var betlayinvoled = _mapper.Map<InvolvedLayBets>(added2);
                    await _OfferService.AddToInvolvedLayBets(Layoffer.Id, betlayinvoled);
                   SendNotification(newlaybet, "MatchingBet");
                    bettoreturn.Add(newlaybet);
                }
            }
           
            return Ok(bettoreturn); 
        }

        [HttpPost(ApiRoutes.Offers.PlaceBackbet, Name = "placebackbet")]
         [Authorize(Roles ="Client")]
        public async Task<IActionResult> PlaceBackOffer([FromBody] List<OfferDto> offer)
        {
            double laibelty=0.00;
            foreach (var item in offer)
            {
                laibelty = laibelty + item.Stake;
            }
         if (await _OfferService.CheckLaiaabilty(laibelty, offer[0].UserId) == false)
                return BadRequest("you don't have enough cash to place this bet");
         else await _accountService.TakeFromBalance(offer[0].UserId, laibelty);
            List<List<Bet>> bettoreturn = new List<List<Bet>>();
            foreach (var item in offer) 
            {
                item.MarketName = item.MarketName.Replace("%2F", "/");
                

                var layexsist = await _OfferService.CheckExsitingLaybets(item.Odds, item.MarketName, item.EventId, item.RunnerName);
                if (layexsist != null)
                {
                    var exsist = await _OfferService.GetLayOffer(layexsist);
                    if (exsist.Liquidity != 0)
                    {
                        if (exsist.Liquidity == item.Stake || exsist.Liquidity > item.Stake)
                        {

                            await _OfferService.TakeFromLayLiquidity(exsist.Id, item.Stake);
                            var matchedbet = _mapper.Map<Bet>(item);
                            var betinvolved = await _OfferService.ManageInvolvedLayBets(exsist.Id, matchedbet);
                            matchedbet.Status = "Closed";
                            matchedbet.RemainingStake = 0;
                            matchedbet.InStake = item.Stake;
                            matchedbet.Lalaibelty = laibelty;
                            matchedbet.Profit = item.Stake * item.Odds;
                            var bettoadd = await _OfferService.AddToBets(matchedbet);
                            await _BetCSer.addtoCounter(bettoadd.EventId);
                            await _OfferService.AddToLayMatchedbet(exsist.Id, bettoadd);
                            betinvolved.Add(bettoadd);
                            SendNotification(betinvolved,"MatchingBet");
                            bettoreturn.Add(betinvolved);
                        }
                        else if (exsist.Liquidity < item.Stake)
                        {
                            var remainingStake = item.Stake - exsist.Liquidity;
                            var matchedbet = _mapper.Map<Bet>(item);
                            var betinvolved = await _OfferService.ManageInvolvedLowLayBets(exsist.Id, matchedbet);
                            matchedbet.InStake = exsist.Liquidity;
                            matchedbet.RemainingStake = 0;
                            matchedbet.Status = "Closed";
                            matchedbet.Lalaibelty = exsist.Liquidity;
                            matchedbet.Profit = exsist.Liquidity * item.Odds;
                            var bettoadd = await _OfferService.AddToBets(matchedbet);
                            await _BetCSer.addtoCounter(bettoadd.EventId);
                            await _OfferService.AddToLayMatchedbet(exsist.Id, bettoadd);
                            var x = _mapper.Map<BackOffer>(item);
                            x.Liquidity = remainingStake;
                            var newBack = await _OfferService.AddBackOffer(x);
                            var c = _mapper.Map<Bet>(x);
                            var added = await _OfferService.AddToBets(c);
                            await _BetCSer.addtoCounter(added.EventId);
                            var betbackinvoled = _mapper.Map<InvolvedBackBets>(added);
                            await _OfferService.AddToInvolvedBackBets(newBack.Id, betbackinvoled);
                            betinvolved.Add(matchedbet);
                            SendNotification(betinvolved,"MatchingBet");
                            bettoreturn.Add(betinvolved);

                        }
                    }
                }
                var backexsist = await _OfferService.CheckExsitingBackbets(item.Odds, item.MarketName, item.EventId, item.RunnerName);
                if (backexsist != null)
                {
                    List<Bet> backbets = new List<Bet>();
                    await _OfferService.AddToBackLiquidity(backexsist, item.Stake);
                    var backbet = _mapper.Map<Bet>(item);
                    backbets.Add(backbet);
                    var added = await _OfferService.AddToBets(backbet);
                    await _BetCSer.addtoCounter(added.EventId);
                    var involved = _mapper.Map<InvolvedBackBets>(added);
                    involved.Status = "open";
                    await _OfferService.AddToInvolvedBackBets(backexsist, involved);
                   SendNotification(backbets,"New Bet Accepted and waiting for matching");
                    bettoreturn.Add(backbets);
                }
                if (layexsist == null && backexsist == null)
                {
                    List<Bet> newbackbet = new List<Bet>();
                    var betoffer = _mapper.Map<BackOffer>(item);
                    var backoffer = await _OfferService.AddBackOffer(betoffer);
                    var bet = _mapper.Map<Bet>(betoffer);
                    newbackbet.Add(bet);
                    var added2 = await _OfferService.AddToBets(bet);
                    await _BetCSer.addtoCounter(added2.EventId);
                    var betlayinvoled = _mapper.Map<InvolvedBackBets>(added2);
                    await _OfferService.AddToInvolvedBackBets(backoffer.Id, betlayinvoled);
                    SendNotification(newbackbet,"New Bet Accepted and waiting for matching");
                    bettoreturn.Add(newbackbet);
                }

            }
           
            return Ok(bettoreturn);
                

        }


        [HttpDelete(ApiRoutes.Offers.Removeoffers, Name = "Removeoffers")]
         

        public async Task<IActionResult> RemoveUnmatchedBet(string userid)

        {
            if (!ModelState.IsValid)
            {
                return new ModelValidator.UnprocessableEntityObjectResult(ModelState);
            }
            await _OfferService.RemoveUnmatchedBet(userid);
            return Ok("unmatched bets Removed");
        }

   


      [HttpGet(ApiRoutes.Offers.getUnmatched, Name = "GetUnmatched")]

      public async  Task<IActionResult> GetUnmatched([FromQuery] string UserId)
      {
          var unmatchedbets = await _OfferService.getunmatched(UserId);
          var result = _mapper.Map<IEnumerable<BetSlipDTO>>(unmatchedbets);
          return Ok(result);
      }

        public async void SendNotification(List<Bet> betinvolved ,string notificationType)
        {

                foreach (var item in betinvolved)
                {
                var userId= item.UserId;
                var usrername = await _accountService.GetusernamebyUserId(userId);
                var h = await _conser.getHconnectionId(usrername);
                var l = await _conser.getLconnectionId(usrername);
                RequestingUser RUser = new RequestingUser()
                {
                    Username=usrername,
   
                };
                await _OfferService.AddNotification(item,notificationType,RUser);
                if (h[0] !="")
                    {
                        foreach(var item2 in h)
                            {
                                await _BetSystemHub.Clients.Client(item2).SendAsync("newMatchedBet",item);  
                            }
                    }
                if (l[0] !="")
                    {
                        foreach(var item2 in l)
                            {
                                await _LiveGameHub.Clients.Client(item2).SendAsync("newMatchedBet",item);  
                            }
                    }

                }
      
        }
            

        

    
    }

}

