using AutoMapper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.DataAccess;
using testtest.dto;
using testtest.dto.Site;
using testtest.helpers;
using testtest.Models;
using testtest.Models.Site;
using testtestdbcontext.AppUserService.Site;

namespace testtest.Service.Site
{
    public class OfferService : IOfferService
    {
        protected IMongoCollection<LayOffer> _Layoffer;
        private readonly IMongoDbContext _context;
        protected IMongoCollection<Bet> _Bet;
        protected IMongoCollection<BackOffer> _Backoffer;
        protected IAccountService _Account;
        protected IMongoCollection<InvolvedLayBets> _involved;
        protected IMongoCollection<InvolvedBackBets> _involvedB;
        protected IMongoCollection<Event> _Event;
        private readonly IMapper _mapper;
        protected IEventServices _eventserv;
        private IMongoCollection<Notification> _notif;
        protected IBetCounterService _betcser;
        
         


        public OfferService(IMongoDbContext context,
         IAccountService accountService, IMapper mapper,
          IEventServices eventserv,
          IBetCounterService betcser
          )
        {
            _context = context;
            _Layoffer = _context.GetCollection<LayOffer>(typeof(LayOffer).Name);
            _Bet = _context.GetCollection<Bet>(typeof(Bet).Name);
            _Backoffer = _context.GetCollection<BackOffer>(typeof(BackOffer).Name);
            _Account = accountService;
            _mapper = mapper;
            _involved= _context.GetCollection<InvolvedLayBets>(typeof(InvolvedLayBets).Name);
            _involvedB= _context.GetCollection<InvolvedBackBets>(typeof(InvolvedBackBets).Name);
            _eventserv = eventserv;
            _notif = _context.GetCollection<Notification>(typeof(Notification).Name);
            _betcser=betcser;

        }

        public async Task<bool> AddToLayLiquidity(string id, double stake)
        {
            var filter = Builders<LayOffer>.Filter.Eq("Id", id);
            var matched = await _Layoffer.Find(filter).FirstAsync();
            var x = matched.Liquidity + stake;
            var update = Builders<LayOffer>.Update.Set("Liquidity", x);
            await _Layoffer.UpdateOneAsync(filter, update);
            return true;
        }
        public async Task<bool> AddToBackLiquidity(string id, double stake)
        {
            var filter = Builders<BackOffer>.Filter.Eq("Id", id);
            var matched = await _Backoffer.Find(filter).FirstAsync();
            var x = matched.Liquidity + stake;
            var update = Builders<BackOffer>.Update.Set("Liquidity", x);
            await _Backoffer.UpdateOneAsync(filter, update);
            return true;
        }



        public async Task<bool> AddToLayMatchedbet(string id, Bet bet)
        {
            var filter = Builders<LayOffer>.Filter.Eq("Id", id);
            var matched = await _Layoffer.Find(filter).FirstAsync();
            if (matched.MatchedBets == null)
            {
                List<Bet> x = new List<Bet> { bet };
                var update = Builders<LayOffer>.Update.Set("MatchedBets", x);
                await _Layoffer.UpdateOneAsync(filter, update);
                return true;
            }
            else
            {
                var oldlist = matched.MatchedBets;
                oldlist.Add(bet);
                var update2 = Builders<LayOffer>.Update.Set("MatchedBets", oldlist);
                await _Layoffer.UpdateOneAsync(filter, update2);

                return true;
            }
        }
        public async Task<bool> AddToBackMatchedbet(string id, Bet bet)
        {
            var filter = Builders<BackOffer>.Filter.Eq("Id", id);
            var matched = await _Backoffer.Find(filter).FirstAsync();
            if (matched.MatchedBets == null)
            {
                List<Bet> x = new List<Bet> { bet };
                var update = Builders<BackOffer>.Update.Set("MatchedBets", x);
                await _Backoffer.UpdateOneAsync(filter, update);
                return true;
            }
            else
            {
                var oldlist = matched.MatchedBets;
                oldlist.Add(bet);
                var update2 = Builders<BackOffer>.Update.Set("MatchedBets", oldlist);
                await _Backoffer.UpdateOneAsync(filter, update2);

                return true;
            }
        }



        public async Task<LayOffer> AddLayOffer(LayOffer offer)
        {

            await _Layoffer.InsertOneAsync(offer);
            return offer;
        }
        public async Task<BackOffer> AddBackOffer(BackOffer offer)
        {

            await _Backoffer.InsertOneAsync(offer);
            return offer;
        }


        
        public async Task<string> CheckExsitingLaybets(double odd, string MarketName, int EventId, string RunnerName)
        {

            var filter1 = Builders<LayOffer>.Filter.Eq("Odds", odd);
            var filter2 = Builders<LayOffer>.Filter.Eq("MarketName", MarketName);
            var filter3 = Builders<LayOffer>.Filter.Eq("EventId", EventId);
            var filter4 = Builders<LayOffer>.Filter.Eq("RunnerName", RunnerName);
            var filter5 = Builders<LayOffer>.Filter.Gt("Liquidity", 0);
            var filter = Builders<LayOffer>.Filter.And(filter1,filter2,filter3, filter4,filter5);
            var matched = await _Layoffer.Find(filter).FirstOrDefaultAsync();
            if (matched != null)
                return matched.Id;
            else return null;

        }
        public async Task<string> CheckExsitingBackbets(double odd, string MarketName, int EventId, string RunnerName)
        {
            var filter1 = Builders<BackOffer>.Filter.Eq("Odds", odd);
            var filter2 = Builders<BackOffer>.Filter.Eq("MarketName", MarketName);
            var filter3 = Builders<BackOffer>.Filter.Eq("EventId", EventId);
            var filter4 = Builders<BackOffer>.Filter.Eq("RunnerName", RunnerName);
            var filter5 = Builders<BackOffer>.Filter.Gt("Liquidity", 0);
            var filter = Builders<BackOffer>.Filter.And(filter1,filter2,filter3, filter4,filter5);

            var matched = await _Backoffer.Find(filter).FirstOrDefaultAsync();
            if (matched != null)
                return matched.Id;
            else return null;



        }



        public async Task<List<LayOffer>> GetLayOffers(int EventId,string MarketName)
        {
           
            List<LayOffer> offerstoreturn = new List<LayOffer>();
            var layoffersscraped = await GetScrapedLayOffers(EventId, MarketName);
            foreach (var item in layoffersscraped)
            {
                var runvalue = item.RunnerName;
                var vis = item.visibility;
                var filter1 = Builders<LayOffer>.Filter.Gt("Liquidity", 0);
                var filter2 = Builders<LayOffer>.Filter.Eq("EventId", EventId);
                var filter3 = Builders<LayOffer>.Filter.Eq("MarketName", MarketName);
                var filter4 = Builders<LayOffer>.Filter.Eq("RunnerName", runvalue);
                var filter = Builders<LayOffer>.Filter.And(filter1, filter2, filter3, filter4);
                var x = _Layoffer.Find(filter).SortByDescending(x => x.Liquidity).Limit(3).ToListAsync();
                if (x.Result.Count != 0)
                {
                    var c = x.Result.ToArray();
                    for (int xx = 0; xx < x.Result.Count; xx++)
                    {
                        // var c = x.Result.ToArray();
                        c[xx].visibility = vis;
                        offerstoreturn.Add(c[xx]);
                    }
                }
                else
                {
                    item.MarketName = MarketName;
                    item.KeepInPlay = true;
                    item.Status = "open";
                    item.Liquidity = 10;
                    item.Side = "lay";
                    item.visibility = vis;
                    item.EventId=EventId;
                    offerstoreturn.Add(item);
                   
                }
            }
            if (MarketName == "Total Goals")
                offerstoreturn.Sort((a, b) => a.RunnerName.CompareTo(b.RunnerName));
            
            return offerstoreturn;

        }
        public async Task<List<BackOffer>> GetBackOffers(int EventId,string MarketName)
        {
            List<BackOffer> offerstoreturn = new List<BackOffer>();
            var backoffersscraped = await GetScrapedBackOffers(EventId, MarketName);
            foreach (var item in backoffersscraped)
            {
                var runvalue = item.RunnerName;
                var vis = item.visibility;
                var filter1 = Builders<BackOffer>.Filter.Gt("Liquidity", 0);
                var filter2 = Builders<BackOffer>.Filter.Eq("EventId", EventId);
                var filter3 = Builders<BackOffer>.Filter.Eq("MarketName", MarketName);
                var filter4 = Builders<BackOffer>.Filter.Eq("RunnerName", runvalue);
                var filter = Builders<BackOffer>.Filter.And(filter1, filter2, filter3, filter4);
                var x = _Backoffer.Find(filter).SortByDescending(x => x.Liquidity).Limit(3).ToListAsync();
                if (x.Result.Count != 0)
                {
                    var c = x.Result.ToArray();
                    for (int xx = 0; xx < x.Result.Count; xx++)
                    {
                        
                        c[xx].visibility = vis;
                        offerstoreturn.Add(c[xx]);
                    }
                }
                else
                {
                    item.MarketName = MarketName;
                    item.KeepInPlay = true;
                    item.Status = "open";
                    item.Liquidity = 10;
                    item.Side = "back";
                    item.visibility = vis;
                     item.EventId=EventId;
                    offerstoreturn.Add(item);
                }
            }
            if (MarketName == "Total Goals")
                offerstoreturn.Sort((a, b) => a.RunnerName.CompareTo(b.RunnerName));
            
            return offerstoreturn;

        }

        public async Task<List<LayOffer>> GetScrapedLayOffers(int EventId, string MarketName)
        {
           
                
            var list =  _eventserv.GetMarketsodds(EventId, MarketName).Result; 
            
            var offers = _mapper.Map<List<LayOffer>>(list);
            return offers;

        }

        public async Task<List<BackOffer>> GetScrapedBackOffers(int EventId, string MarketName)
        {
            var list = _eventserv.GetMarketsodds(EventId, MarketName).Result;

            var offers = _mapper.Map<List<BackOffer>>(list);
            return offers;

        }


        public async Task<LayOffer> GetLayOffer(string id)
        {
            var filter = Builders<LayOffer>.Filter.Eq("Id", id);
            var matched = await _Layoffer.Find(filter).FirstOrDefaultAsync();
            return matched;

        }
        public async Task<BackOffer> GetBackOffer(string id)
        {
            var filter = Builders<BackOffer>.Filter.Eq("Id", id);
            var matched = await _Backoffer.Find(filter).FirstOrDefaultAsync();
            return matched;

        }




        public async Task<bool> TakeFromLayLiquidity(string id, double stake)
        {
            var matched = await _Layoffer.Find(x => x.Id == id).FirstOrDefaultAsync();
            var x = matched.Liquidity - stake;
            var filter = Builders<LayOffer>.Filter.Eq("Id", id);
            var update = Builders<LayOffer>.Update.Set("Liquidity", x);
            await _Layoffer.UpdateOneAsync(filter, update);
            return true;
        }
        public async Task<bool> TakeFromBackLiquidity(string id, double stake)
        {
            var matched = await _Backoffer.Find(x => x.Id == id).FirstOrDefaultAsync();
            var x = matched.Liquidity - stake;
            var filter = Builders<BackOffer>.Filter.Eq("Id", id);
            var update = Builders<BackOffer>.Update.Set("Liquidity", x);
            await _Backoffer.UpdateOneAsync(filter, update);
            return true;
        }


         public async Task<bool> RemoveUnmatchedBet(string userid)
        {
           
            var filter1 = Builders<Bet>.Filter.Eq("UserId", userid);
            var filter2 = Builders<Bet>.Filter.Eq("Status", "open");
            var filter3 =Builders<Bet>.Filter.Eq("Side", "back");
            var filter = Builders<Bet>.Filter.And(filter1, filter2,filter3);
            var x = await _Bet.Find(filter).ToListAsync();
            foreach (var ite in x)
                {
                    var betid = ite.Id;
                    var invo = await _involvedB.Find(x => x.BetId == betid).FirstOrDefaultAsync();
                    var backoffer = await _Backoffer.Find(z => z.Id == invo.OfferId).FirstOrDefaultAsync();
                    var oldLiquidity = backoffer.Liquidity;
                    var update = Builders<BackOffer>.Update.Set("Liquidity", oldLiquidity - ite.RemainingStake);
                    await _Backoffer.UpdateOneAsync(X => X.Id == backoffer.Id, update);
                    await _involvedB.DeleteOneAsync(z => z.BetId == betid);
                    await _Bet.DeleteOneAsync(z => z.Id == betid);
                    await _betcser.takefromCounter(ite.EventId);
                    await _Account.AddDepositToBalance(ite.UserId,ite.RemainingStake);
                }
            var filter4 = Builders<Bet>.Filter.Eq("UserId", userid);
            var filter5 = Builders<Bet>.Filter.Eq("Status", "open");
            var filter6 =Builders<Bet>.Filter.Eq("Side", "lay");
            var filter7 = Builders<Bet>.Filter.And(filter1, filter2,filter3);
            var y = await _Bet.Find(filter7).ToListAsync();
            foreach (var item in y)
                {
                    var betid = item.Id;
                    var invo = await _involved.Find(x => x.BetId == betid).FirstOrDefaultAsync();
                    var layoffer = await _Layoffer.Find(z => z.Id == invo.OfferId).FirstOrDefaultAsync();
                    var oldLiquidity = layoffer.Liquidity;
                    var update = Builders<LayOffer>.Update.Set("Liquidity", oldLiquidity - item.RemainingStake);
                    await _Layoffer.UpdateOneAsync(X => X.Id == layoffer.Id, update);
                    await _involved.DeleteOneAsync(z => z.BetId == betid);
                    await _Bet.DeleteOneAsync(z => z.Id == betid);
                    await _betcser.takefromCounter(item.EventId);
                    await _Account.AddDepositToBalance(item.UserId,item.RemainingStake*item.Odds);

                }
     
            return true;
         }
        

        public async Task<bool> AddToInvolvedLayBets(string id, InvolvedLayBets involvedBet)
        {
            involvedBet.OfferId = id;
            involvedBet.Status = "open";
            await _involved.InsertOneAsync(involvedBet);
            return true;
        }
        public async Task<bool> AddToInvolvedBackBets(string id, InvolvedBackBets involvedBet)
        {
            involvedBet.OfferId = id;
            involvedBet.Status = "open";
            await _involvedB.InsertOneAsync(involvedBet);
            return true;
        }



        public async Task<Bet> AddToBets(Bet bet)
        {
            await _Bet.InsertOneAsync(bet);
            return bet;
        }

        public async Task<bool> CheckLaiaabilty(double laibelty, string id)
        {
            
           var balance = await _Account.GetBalancebyID(id);
            if (laibelty <= balance)
                return true;
            else return false;

        }


        public async Task<List<Bet>> ManageInvolvedBackBets(string Offerid, Bet bet)
        {
            List<Bet> y = new List<Bet>();
            var x = bet.RemainingStake;
            var filter1 = Builders<InvolvedBackBets>.Filter.Eq("OfferId", Offerid);
            IEnumerable<string> state = new string[] { "open", "partially" };
            var filter2 = Builders<InvolvedBackBets>.Filter.In("Status", state);

            while (x != 0)
            {
                var filter4 = Builders<InvolvedBackBets>.Filter.Eq("RemainingStake", x);
                var filter = Builders<InvolvedBackBets>.Filter.And(filter1, filter4, filter2);
                var involoved = await _involvedB.Find(filter).FirstOrDefaultAsync();
                if (involoved != null)
                {

                    var update = Builders<Bet>.Update.Set("Status", "Closed")
                                                     .Set("InStake", x)
                                                     .Set("RemainingStake", 0)
                                                     .Set("Lalaibelty",x)
                                                     .Set("Profit",x*bet.Odds);
                    await _Bet.UpdateOneAsync(x => x.Id == involoved.BetId, update);

                    var backbettoadd = await _Bet.Find(x => x.Id == involoved.BetId).FirstOrDefaultAsync();
                   
                    var update1 = Builders<InvolvedBackBets>.Update.Set("Status", "Closed")
                                                                  .Set("RemainingStake", 0);
                    await _involvedB.UpdateOneAsync(x => x.BetId == involoved.BetId, update1);
                    x = 0;
                    y.Add(backbettoadd);

                }
                else
                {
                    var filter6 = Builders<InvolvedBackBets>.Filter.Gt("RemainingStake", x);
                    var filter8 = Builders<InvolvedBackBets>.Filter.And(filter1, filter6, filter2);
                    var Involved2 = await _involvedB.Find(filter8).FirstOrDefaultAsync();
                    if (Involved2 != null)
                    {



                        var NewRemaining = Involved2.RemainingStake - x;
                        var update = Builders<InvolvedBackBets>.Update.Set("Status", "partially")
                                                                      .Set("RemainingStake", NewRemaining);
                        await _involvedB.UpdateOneAsync(x => x.BetId == Involved2.BetId, update);

                        
                        var update1 = Builders<Bet>.Update.Set("Status", "open")
                                                          .Set("RemainingStake", NewRemaining);
                        Bet newbet = new Bet()
                        {
                            EventId = bet.EventId,
                            InStake = x,
                            RunnerName = bet.RunnerName,
                            MarketName = bet.MarketName,
                            RemainingStake = 0,
                            Odds = bet.Odds,
                            Side = "back",
                            UserId = Involved2.UserId,
                            Status = "closed",
                            CreationDate = DateTime.Now,
                            Teams = bet.Teams,
                            Profit = x * bet.Odds,
                            Lalaibelty = x
                        };
                        var betnew = await AddToBets(newbet);
                        await _betcser.addtoCounter(betnew.EventId);
                        await _Bet.UpdateOneAsync(x => x.Id == Involved2.BetId, update1);
                        
                        x = 0;
                        y.Add(betnew);



                    }


                    else
                    {
                        var filter7 = Builders<InvolvedBackBets>.Filter.Lt("RemainingStake", x);
                        var filter9 = Builders<InvolvedBackBets>.Filter.And(filter1, filter7, filter2);
                        var Involved3 = await _involvedB.Find(filter9).FirstOrDefaultAsync();
                        if (Involved3 != null)
                        {
                            var z = x - Involved3.RemainingStake;
                         
                            var update = Builders<Bet>.Update.Set("Status", "closed")
                                                             .Set("InStake", Involved3.RemainingStake)
                                                             .Set("RemainingStake", 0)
                                                             .Set("Lalaibelty", x)
                                                             .Set("Profit", x * bet.Odds);
                            await _Bet.UpdateOneAsync(x => x.Id == Involved3.BetId, update);
                            var bettoreturn = await _Bet.Find(x => x.Id == Involved3.BetId).FirstOrDefaultAsync();
                          
                            var update1 = Builders<InvolvedBackBets>.Update.Set("Status", "Closed")
                                                                          .Set("RemainingStake", 0);
                            await _involvedB.UpdateOneAsync(x => x.BetId == Involved3.BetId, update1);
                            x = z;

                            y.Add(bettoreturn);


                        }


                    }

                }
            }
            return y;

        }

        public async Task<List<Bet>> ManageInvolvedLowBackBets(string Offerid, Bet bet)
        {
            List<Bet> y = new List<Bet>();

            var filter1 = Builders<InvolvedBackBets>.Filter.Eq("OfferId", Offerid);
            IEnumerable<string> state = new string[] { "open", "partially" };
            var filter2 = Builders<InvolvedBackBets>.Filter.In("Status", state);
            var filter3 = Builders<InvolvedBackBets>.Filter.Lt("RemainingStake", bet.RemainingStake);
            var filter4 = Builders<InvolvedBackBets>.Filter.And(filter1, filter3, filter2);
            var ex = await _Backoffer.Find(x => x.Id == Offerid).FirstOrDefaultAsync();
            var c = ex.Liquidity;
            while (c > 0)
            {
                var involoved = await _involvedB.Find(filter4).FirstOrDefaultAsync();
                var update = Builders<Bet>.Update.Set("Status", "Closed")
                                                     .Set("InStake", involoved.RemainingStake)
                                                     .Set("RemainingStake", 0)
                                                     .Set("Lalaibelty", involoved.RemainingStake)
                                                     .Set("Profit", involoved.RemainingStake * bet.Odds);
                await _Bet.UpdateOneAsync(x => x.Id == involoved.BetId, update);

                var backbettoadd = await _Bet.Find(x => x.Id == involoved.BetId).FirstOrDefaultAsync();
                c = ex.Liquidity - backbettoadd.InStake;
                var update1 = Builders<InvolvedBackBets>.Update.Set("Status", "Closed")
                                                                     .Set("RemainingStake", 0);
                await _involvedB.UpdateOneAsync(x => x.BetId == involoved.BetId, update1);

                y.Add(backbettoadd);
            }
            var update2 = Builders<BackOffer>.Update.Set("Liquidity", 0);
            await _Backoffer.UpdateOneAsync(x => x.Id == Offerid, update2);
            return y;
        }
        public async Task<List<Bet>> ManageInvolvedLayBets(string Offerid, Bet bet)
        {
            List<Bet> y = new List<Bet>();
            var x = bet.RemainingStake;
            var filter1 = Builders<InvolvedLayBets>.Filter.Eq("OfferId", Offerid);
            IEnumerable<string> state = new string[] { "open", "partially" };
            var filter2 = Builders<InvolvedLayBets>.Filter.In("Status", state);
         
            while (x != 0)
            {
                var filter4 = Builders<InvolvedLayBets>.Filter.Eq("RemainingStake", x);
                var filter = Builders<InvolvedLayBets>.Filter.And(filter1, filter4, filter2);
                var involoved = await _involved.Find(filter).FirstOrDefaultAsync();
                if (involoved != null)
                {

                    var update = Builders<Bet>.Update.Set("Status", "Closed")
                                                     .Set("InStake", x)
                                                     .Set("RemainingStake", 0)
                                                     .Set("Lalaibelty", x*bet.Odds)
                                                     .Set("Profit", x );

                    await _Bet.UpdateOneAsync(x => x.Id == involoved.BetId, update);

                    var backbettoadd = await _Bet.Find(x => x.Id == involoved.BetId).FirstOrDefaultAsync();
                    var update1 = Builders<InvolvedLayBets>.Update.Set("Status", "Closed")
                                                                  .Set("RemainingStake", 0);
                    await _involved.UpdateOneAsync(x => x.BetId == involoved.BetId, update1);
                    x = 0;
                    y.Add(backbettoadd);

                }
                else
                {
                    var filter6 = Builders<InvolvedLayBets>.Filter.Gt("RemainingStake", x);
                    var filter8 = Builders<InvolvedLayBets>.Filter.And(filter1, filter6, filter2);
                    var Involved2 = await _involved.Find(filter8).FirstOrDefaultAsync();
                    if (Involved2 != null)
                    {



                        var NewRemaining = Involved2.RemainingStake - x;
                        var update = Builders<InvolvedLayBets>.Update.Set("Status", "partially")
                                                                      .Set("RemainingStake", NewRemaining);
                        await _involved.UpdateOneAsync(x => x.BetId == Involved2.BetId, update);

                        var update1 = Builders<Bet>.Update.Set("Status", "open")
                                                          .Set("RemainingStake", NewRemaining);
                        Bet newbet = new Bet()
                        {
                            EventId = bet.EventId,
                            InStake = x,
                            RunnerName = bet.RunnerName,
                            MarketName = bet.MarketName,
                            RemainingStake = 0,
                            Odds = bet.Odds,
                            Side = "lay",
                            UserId = Involved2.UserId,
                            Status = "Closed",
                            CreationDate = DateTime.Now,
                            Teams = bet.Teams,
                            Profit = x ,
                            Lalaibelty = x * bet.Odds
                        };
                        var betnew = await AddToBets(newbet);
                        await _betcser.addtoCounter(betnew.EventId);
                        await _Bet.UpdateOneAsync(x => x.Id == Involved2.BetId, update1);
                      
                        x = 0;
                        y.Add(betnew);



                    }


                    else
                    {
                        var filter7 = Builders<InvolvedLayBets>.Filter.Lt("RemainingStake", x);
                        var filter9 = Builders<InvolvedLayBets>.Filter.And(filter1, filter7, filter2);
                        var Involved3 = await _involved.Find(filter9).FirstOrDefaultAsync();
                        if (Involved3 != null)
                        {
                            var z = x - Involved3.RemainingStake;
                     
                            var update = Builders<Bet>.Update.Set("Status", "Closed")
                                                             .Set("InStake", Involved3.RemainingStake)
                                                             .Set("RemainingStake", 0)
                                                             .Set("Lalaibelty", x * bet.Odds)
                                                             .Set("Profit", x);

                            await _Bet.UpdateOneAsync(x => x.Id == Involved3.BetId, update);
                            var bettoreturn = await _Bet.Find(x => x.Id == Involved3.BetId).FirstOrDefaultAsync();
                           
                            var update1 = Builders<InvolvedLayBets>.Update.Set("Status", "Closed")
                                                                          .Set("RemainingStake", 0);
                            await _involved.UpdateOneAsync(x => x.BetId == Involved3.BetId, update1);
                            x = z;

                            y.Add(bettoreturn);


                        }


                    }

                }
            }
            return y;

        }

        public async Task<List<Bet>> ManageInvolvedLowLayBets(string Offerid, Bet bet)
        {
            List<Bet> y = new List<Bet>();
           
            var filter1 = Builders<InvolvedLayBets>.Filter.Eq("OfferId", Offerid);
            IEnumerable<string> state = new string[] { "open", "partially" };
            var filter2 = Builders<InvolvedLayBets>.Filter.In("Status", state);
            var filter3 = Builders<InvolvedLayBets>.Filter.Lt("RemainingStake", bet.RemainingStake);
            var filter4 = Builders<InvolvedLayBets>.Filter.And(filter1, filter3, filter2);
            var ex = await _Layoffer.Find(x => x.Id == Offerid).FirstOrDefaultAsync();
            var c = ex.Liquidity;
            while (c > 0)
            {
                var involoved = await _involved.Find(filter4).FirstOrDefaultAsync();
                var update = Builders<Bet>.Update.Set("Status", "Closed")
                                                     .Set("InStake", involoved.RemainingStake)
                                                     .Set("RemainingStake", 0)
                                                     .Set("Lalaibelty", involoved.RemainingStake * bet.Odds)
                                                     .Set("Profit", involoved.RemainingStake);
                await _Bet.UpdateOneAsync(x => x.Id == involoved.BetId, update);

                var backbettoadd = await _Bet.Find(x => x.Id == involoved.BetId).FirstOrDefaultAsync();
                 c= ex.Liquidity - backbettoadd.InStake;
                var update1 = Builders<InvolvedLayBets>.Update.Set("Status", "Closed")
                                                                     .Set("RemainingStake", 0);
                await _involved.UpdateOneAsync(x => x.BetId == involoved.BetId, update1);
                
                y.Add(backbettoadd);
            }
            var update2 = Builders<LayOffer>.Update.Set("Liquidity", 0);
            await _Layoffer.UpdateOneAsync(x => x.Id == Offerid, update2);
            return y;
        }

           

      

        public async Task<bool> ZeroBLequdity(string id)
        {
            var update = Builders<BackOffer>.Update.Set("Liquidity", 0);
            await _Backoffer.UpdateOneAsync(x => x.Id == id, update);
            return true;
        }
        public async Task<bool> ZeroLLequdity(string id)
        {
            var update = Builders<LayOffer>.Update.Set("Liquidity", 0);
            await _Layoffer.UpdateOneAsync(x => x.Id == id, update);
            return true;
        }

        public async Task AddNotification(Bet bets, string type, RequestingUser user)
        {
            
            
            
                Notification newNotification = new Notification();
                newNotification.Date = DateTime.Now;
                newNotification.UserName = user.Username;
                newNotification.UserId = bets.UserId;
                if (type == "MatchingBet")
                {
                    newNotification.Title = "Bet Matched";
                    newNotification.Type =bets.Side+"MatchingBet";
                    newNotification.Details = "New Bet Matched with odds of " +bets.Odds+ " and Stake of" +bets.InStake ;
                    newNotification.Amount= bets.InStake.ToString();
                }
                if (type == "New Bet Accepted and waiting for matching")
                {
                    newNotification.Title = "New Bet";
                    newNotification.Type =bets.Side+" Bet";
                    newNotification.Details = "New Bet Accepted with odds of " +bets.Odds+ " and Stake of" +bets.RemainingStake ;
                    newNotification.Amount= bets.RemainingStake.ToString();
                }
                    var userid = bets.UserId;
                newNotification.NotificationReceivers = new List<NotificationReceiver>();
               
                    newNotification.NotificationReceivers.Add(new NotificationReceiver
                    {
                        IsDeleted = false,
                        ReceiverId = userid,
                    });
                await _notif.InsertOneAsync(newNotification);
    
        }

        public async Task<List<OfferResultDto>> Getoffers(int EventId,string MarketName)
        {
            List<LayOffer> layoffers = new List<LayOffer>();
            MarketName = MarketName.Replace("%2F", "/");
            layoffers = await GetLayOffers(EventId, MarketName);
            var result = _mapper.Map<List<OfferResultDto>>(layoffers);
            List<BackOffer> backoffers = new List<BackOffer>();
         
            backoffers = await GetBackOffers(EventId, MarketName);
            var result2 = _mapper.Map<List<OfferResultDto>>(backoffers);
            result.AddRange(result2);
           
           
            return result;
        }
     
        public async Task<List<Bet>> getunmatched(string UserID)
          {

            var f1 = Builders<Bet>.Filter.Eq("UserId",UserID) ;
            var f2 = Builders<Bet>.Filter.Eq("Status","open"); 
            var f3 = Builders<Bet>.Filter.And(f1,f2);
            var listoFunmatchedBets = await _Bet.Find(f3).ToListAsync();
            return listoFunmatchedBets;
         }

         public async Task<List<Bet>> getmatched(string UserID)
          {

            var f1 = Builders<Bet>.Filter.Eq("UserId",UserID) ;
            var f2 = Builders<Bet>.Filter.Eq("Status","Closed"); 
            var f3 = Builders<Bet>.Filter.And(f1,f2);
            var listoFmatchedBets = await _Bet.Find(f3).ToListAsync();
            return listoFmatchedBets;
         }

    }
}
