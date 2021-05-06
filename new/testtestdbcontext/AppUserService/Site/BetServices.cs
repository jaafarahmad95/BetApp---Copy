using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.DataAccess;
using testtest.dto.Site;
using testtest.Models;
using testtest.Models.Site;
using testtestdbcontext.AppUserService.Site;
using testtestdbcontext.testtest.Models.Site;

namespace testtest.Service.Site
{
    public class BetServices : IBetServices 
    {   protected IMongoCollection<LayOffer> _Layoffer;
        protected IMongoCollection<BackOffer> _Backoffer;
        protected IMongoCollection<InvolvedLayBets> _involved;
        protected IMongoCollection<InvolvedBackBets> _involvedB;
        protected IMongoCollection<Bet> _bet;
        protected IMongoCollection<Event> _Event;
        protected IMongoCollection<Account> _account;
        private readonly IMongoDbContext _context;
        private readonly IOfferService _Offer;
        private readonly IAccountService _AccountS;
        private readonly IBetCounterService _count;
        protected IMongoCollection<Ended> _end ;
        protected IMongoCollection<BetCounter> _BetCountCol;
        public BetServices(IMongoDbContext context,IOfferService offerService,
        IAccountService AccountS,IBetCounterService count,
         IBetCounterService betcser)
        {
            _context = context;
            _bet= context.GetCollection<Bet>(typeof(Bet).Name);
            _Offer = offerService;
            _Event= context.GetCollection<Event>(typeof(Event).Name);
            _account = context.GetCollection<Account>(typeof(Account).Name);
            _AccountS= AccountS;
            _count= count ;
            _BetCountCol = context.GetCollection<BetCounter>(typeof(BetCounter).Name);;
            _Layoffer = _context.GetCollection<LayOffer>(typeof(LayOffer).Name);
            _Backoffer = _context.GetCollection<BackOffer>(typeof(BackOffer).Name);
            _involved= _context.GetCollection<InvolvedLayBets>(typeof(InvolvedLayBets).Name);
            _involvedB= _context.GetCollection<InvolvedBackBets>(typeof(InvolvedBackBets).Name);
            _end = _context.GetCollection<Ended>(typeof(Ended).Name);
        }
        

       
        public async Task<bool> SattelBets()
        {    
            
          
            var listofEndedEvents = await _end.Find(x => true).ToListAsync();
            if(listofEndedEvents !=null)
            {
               foreach (var item in listofEndedEvents)
             {   
                var fb1= Builders<Bet>.Filter.Eq("EventId" ,item.EventId.ToString());
                var fb2=Builders<Bet>.Filter.Eq("Status","Closed");
                var filter2 = Builders<Bet>.Filter.And(fb1, fb2);
                var EventsBets = await _bet.Find(filter2).ToListAsync();
                if (EventsBets.Count != 0)
                {  
                  foreach (var item2 in EventsBets)
                    {   var fb3= Builders<Bet>.Filter.Eq("Id",item2.Id);
                        var fb4 = Builders<Account>.Filter.Eq("UserId",item2.UserId);
                      if(item2.Side=="back")
                        {
                          switch (item2.MarketName)
                            {
                              case "Match Result":
                                if(item2.RunnerName == item.FinalResult)
                                   {
                                    var update = Builders<Bet>.Update.Set("Status","Won");
                                    await _bet.UpdateOneAsync(fb3,update);
                                    var winnings= item2.Profit;
                                    await _AccountS.AddToBalance(item2.UserId,winnings);
                                    await _count.takefromCounter(item2.EventId);
                                    continue;
                                   }
                                else
                                  {
                                    var update = Builders<Bet>.Update.Set("Status","Lost");
                                    await _bet.UpdateOneAsync(fb3,update);
                                    await _count.takefromCounter(item2.EventId);
                                    continue;
                                  }
                              
                              case "Double Chance":
                              
                                  if(item2.RunnerName.Contains(item.FinalResult))
                                    {
                                      var update = Builders<Bet>.Update.Set("Status","Won");
                                      await _bet.UpdateOneAsync(fb3,update);
                                      var winnings= item2.Profit;
                                      await _AccountS.AddToBalance(item2.UserId,winnings);
                                      await _count.takefromCounter(item2.EventId);
                                      continue;
                                    }
                                  
                                  else
                                    {
                                      var update = Builders<Bet>.Update.Set("Status","Lost");
                                      await _bet.UpdateOneAsync(fb3,update);
                                      await _count.takefromCounter(item2.EventId);
                                      continue;
                                    }
                              
                    
                              case " Both Teams to Score":
                                  if(item.first_team_goals != 0 && item.second_team_goals != 0 && item2.RunnerName == "Yes")
                                    {
                                      var update = Builders<Bet>.Update.Set("Status","Won");
                                      await _bet.UpdateOneAsync(fb3,update);
                                      var winnings= item2.Profit;
                                      await _AccountS.AddToBalance(item2.UserId,winnings);
                                      await _count.takefromCounter(item2.EventId);
                                     continue;
                                    }

                                  if(item.first_team_goals == 0 || item.second_team_goals == 0 && item2.RunnerName == "No")
                                    {
                                      var update = Builders<Bet>.Update.Set("Status","Won");
                                      await _bet.UpdateOneAsync(fb3,update);
                                      var winnings= item2.Profit;
                                      await _AccountS.AddToBalance(item2.UserId,winnings);
                                      await _count.takefromCounter(item2.EventId);
                                      continue;
                                    }

                                  else
                                    {
                                      var update = Builders<Bet>.Update.Set("Status","Lost");
                                      await _bet.UpdateOneAsync(fb3,update);
                                      await _count.takefromCounter(item2.EventId);
                                      continue;
                                    }
                              
                              case "Total Goals":
                                  if(item2.RunnerName == "Under 0,5" && item.Goals_Sum == 0)
                                    {
                                      var update = Builders<Bet>.Update.Set("Status","Won");
                                      await _bet.UpdateOneAsync(fb3,update);
                                      var winnings= item2.Profit;
                                      await _AccountS.AddToBalance(item2.UserId,winnings);
                                      await _count.takefromCounter(item2.EventId);
                                      continue;
                                    }
                                  if(item2.RunnerName == "Under 1,5" && item.Goals_Sum < 2)
                                    {
                                      var update = Builders<Bet>.Update.Set("Status","Won");
                                      await _bet.UpdateOneAsync(fb3,update);
                                      var winnings= item2.Profit;
                                      await _AccountS.AddToBalance(item2.UserId,winnings);
                                      await _count.takefromCounter(item2.EventId);
                                      continue;
                                    }
                            if(item2.RunnerName == "Under 2,5" && item.Goals_Sum < 3)
                              {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                              }
                            if(item2.RunnerName == "Under 3,5" && item.Goals_Sum < 4)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 4,5" && item.Goals_Sum < 5)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 5,5" && item.Goals_Sum < 6)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 6,5" && item.Goals_Sum < 7)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 7,5" && item.Goals_Sum < 8)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 8,5" && item.Goals_Sum < 9)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 9,5" && item.Goals_Sum < 10)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 10,5" && item.Goals_Sum < 11)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 0,5" && item.Goals_Sum > 0)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 1,5" && item.Goals_Sum > 1)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 2,5" && item.Goals_Sum > 2)
                              {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                              }
                            if(item2.RunnerName == "Over 3,5" && item.Goals_Sum > 3)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 4,5" && item.Goals_Sum > 4)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 5,5" && item.Goals_Sum > 5)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 6,5" && item.Goals_Sum > 6)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 7,5" && item.Goals_Sum > 7)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 8,5" && item.Goals_Sum > 8)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 9,5" && item.Goals_Sum > 9)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 10,5" && item.Goals_Sum > 10)
                             {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            else
                              {
                                 var update = Builders<Bet>.Update.Set("Status","Lost");
                                await _bet.UpdateOneAsync(fb3,update);
                                await _count.takefromCounter(item2.EventId);
                                continue;
                              }
                        default:
                              continue;
                      }
                        }
                      if(item2.Side=="lay")
                        {
                            switch (item2.MarketName)
                      {
                        case "Match Result":
                              if(item2.RunnerName == item.FinalResult)
                              {
                                var update = Builders<Bet>.Update.Set("Status","Lost");
                                await _bet.UpdateOneAsync(fb3,update);
                                await _count.takefromCounter(item2.EventId);
                                continue;
                              }
                            else
                               {
                                var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                await _AccountS.AddToBalance(item2.UserId,winnings);
                                await _count.takefromCounter(item2.EventId);
                                continue;
                               }
                              
                        case "Double Chance":
                            var first_equals = item.FinalResult + " or X";
						                var second_equals = "X or " + item.FinalResult ;
						               // var first_second = item.participants.first_name + " or " + item.participants.second_name ;
                            if(!(item2.RunnerName.Contains(item.FinalResult)))   
                               {
                                var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit+ item2.Lalaibelty ;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                               }
                            
                            else
                              {
                                 var update = Builders<Bet>.Update.Set("Status","Lost");
                                await _bet.UpdateOneAsync(fb3,update);
                                await _count.takefromCounter(item2.EventId);
                                  continue;
                              }
                              
                    
                        case " Both Teams to Score":

                          if(item.first_team_goals != 0 && item.second_team_goals != 0 && item2.RunnerName == "No")
                            {
                                var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit+ item2.Lalaibelty ;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }

                          if(item.first_team_goals == 0 || item.second_team_goals == 0 && item2.RunnerName == "Yes")
                          {
                                var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit+ item2.Lalaibelty ;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                 continue;
                          }

                          else
                               {
                                 var update = Builders<Bet>.Update.Set("Status","Lost");
                                await _bet.UpdateOneAsync(fb3,update);
                                await _count.takefromCounter(item2.EventId);
                                continue;
                              }
                              
                          
                        case "Total Goals":
                            if(item2.RunnerName == "Under 0,5" && item.Goals_Sum > 0)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 1,5" && item.Goals_Sum > 1)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit+ item2.Lalaibelty ;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 2,5" && item.Goals_Sum > 2)
                              {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                              }
                            if(item2.RunnerName == "Under 3,5" && item.Goals_Sum >3)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 4,5" && item.Goals_Sum >4)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit+ item2.Lalaibelty ;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 5,5" && item.Goals_Sum >5)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit+ item2.Lalaibelty ;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 6,5" && item.Goals_Sum >6)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 7,5" && item.Goals_Sum >7)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 8,5" && item.Goals_Sum >8)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 9,5" && item.Goals_Sum >9)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit+ item2.Lalaibelty ;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Under 10,5" && item.Goals_Sum >10)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 0,5" && item.Goals_Sum == 0)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 1,5" && item.Goals_Sum <2 )
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit+ item2.Lalaibelty ;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 2,5" && item.Goals_Sum <3)
                              {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                              }
                            if(item2.RunnerName == "Over 3,5" && item.Goals_Sum <4)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 4,5" && item.Goals_Sum <5)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 5,5" && item.Goals_Sum < 6)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 6,5" && item.Goals_Sum < 7)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 7,5" && item.Goals_Sum < 8)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 8,5" && item.Goals_Sum < 9)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 9,5" && item.Goals_Sum < 10)
                            {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit + item2.Lalaibelty;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            if(item2.RunnerName == "Over 10,5" && item.Goals_Sum < 11)
                             {
                              var update = Builders<Bet>.Update.Set("Status","Won");
                                await _bet.UpdateOneAsync(fb3,update);
                                var winnings= item2.Profit+ item2.Lalaibelty ;
                                 await _AccountS.AddToBalance(item2.UserId,winnings);
                                 await _count.takefromCounter(item2.EventId);
                                continue;
                            }
                            else
                              {
                                 var update = Builders<Bet>.Update.Set("Status","Lost");
                                await _bet.UpdateOneAsync(fb3,update);
                                await _count.takefromCounter(item2.EventId);
                                continue;
                              }
                        default:
                              continue;
                      }
                        }
                    }
                }
                
                var remainingcounter = await _BetCountCol.Find(y => y.EventId == item.EventId.ToString()).FirstOrDefaultAsync();
                if(remainingcounter!=null)
                {
                     var remcount = remainingcounter.betcounter;
                  if (remcount == 0)
                  {
                  var f = Builders<Ended>.Filter.Eq("EventId",item.EventId);
                  var update = Builders<Ended>.Update.Set("Bet_checked",true);
                  await   _end.UpdateOneAsync(f,update);
                  }
                  else if (remcount != 0) 
                  {
                    var f1 = Builders<Bet>.Filter.Eq("EventId" ,item.EventId.ToString());
                    var fu2 = Builders<Bet>.Filter.Eq("Status","open");
                    var filterun= Builders<Bet>.Filter.And(f1,fu2);
                    var unmatched = _bet.Find(filterun).ToListAsync().Result;
                     foreach (var item2 in unmatched)
                       { var betid = item2.Id;
                         if (item2.Side == "lay")
                     {
                           var invo = await _involved.Find(x => x.BetId == betid).FirstOrDefaultAsync();
                           var layoffer = await _Layoffer.Find(z => z.Id == invo.OfferId).FirstOrDefaultAsync();
                           var oldLiquidity = layoffer.Liquidity;
                           var update = Builders<LayOffer>.Update.Set("Liquidity", oldLiquidity - item2.RemainingStake);
                           await _Layoffer.UpdateOneAsync(X => X.Id == layoffer.Id, update);
                           await _involved.DeleteOneAsync(z => z.BetId == betid);
                           await _bet.DeleteOneAsync(z => z.Id == betid);
                           await _count.takefromCounter(item2.EventId);
                           await _AccountS.AddToBalance(item2.UserId,item2.RemainingStake*item2.Odds);
                     }
                          if(item2.Side=="back")
                     {
                       var invo = await _involvedB.Find(x => x.BetId == betid).FirstOrDefaultAsync();
                       var backoffer = await _Backoffer.Find(z => z.Id == invo.OfferId).FirstOrDefaultAsync();
                       var oldLiquidity = backoffer.Liquidity;
                       var update = Builders<BackOffer>.Update.Set("Liquidity", oldLiquidity - item2.RemainingStake);
                       await _Backoffer.UpdateOneAsync(X => X.Id == backoffer.Id, update);
                       await _involvedB.DeleteOneAsync(z => z.BetId == betid);
                       await _bet.DeleteOneAsync(z => z.Id == betid);
                       await _count.takefromCounter(item2.EventId);
                       await _AccountS.AddToBalance(item2.UserId,item2.RemainingStake);


                     }
                       }
                    var f11 = Builders<Ended>.Filter.Eq("EventId",item.EventId);
                    var update1 = Builders<Ended>.Update.Set("Bet_checked",true);
                    await   _end.UpdateOneAsync(f11,update1);  

                  }
               
                }
            }
              
            }
           
            return true;
        }
        
   
    }
}




