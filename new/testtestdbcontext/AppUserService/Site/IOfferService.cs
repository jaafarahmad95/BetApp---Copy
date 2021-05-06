using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.dto;
using testtest.dto.Site;
using testtest.helpers;
using testtest.Models;
using testtest.Models.Site;

namespace testtest.Service.Site
{
    public interface IOfferService
    {
        Task<List<LayOffer>> GetLayOffers(int EventId,string MarketName);
        Task<List<BackOffer>> GetBackOffers(int EventId, string MarketName);

        Task<List<LayOffer>> GetScrapedLayOffers(int EventId, string MarketName);
        Task<List<BackOffer>> GetScrapedBackOffers(int EventId, string MarketName);

        Task<bool> AddToLayLiquidity(string id, double stake);
        Task<bool> AddToBackLiquidity(string id, double stake);

        Task<bool> TakeFromLayLiquidity(string id, double stake);
        Task<bool> TakeFromBackLiquidity(string id, double stake);

        Task<bool> AddToLayMatchedbet(string id , Bet bet);
        Task<bool> AddToBackMatchedbet(string id, Bet bet);

        Task<string> CheckExsitingLaybets(double odd, string MarketName , int EventId,string RunnerName);
        Task<string> CheckExsitingBackbets(double odd,string MarketName , int EventId, string RunnerName);


        Task<LayOffer> AddLayOffer(LayOffer offer);
        Task<BackOffer> AddBackOffer(BackOffer offer);

        Task<BackOffer> GetBackOffer(string id);
        Task<LayOffer> GetLayOffer(string id);

       


        Task<Bet> AddToBets( Bet bet);
        Task<bool> CheckLaiaabilty(double laibelty, string id);

        Task<bool> RemoveUnmatchedBet(string userid);
     

        Task<bool> AddToInvolvedLayBets(string id, InvolvedLayBets involvedBet);
        Task<bool> AddToInvolvedBackBets(string id, InvolvedBackBets involvedBet);

        Task<List<Bet>> ManageInvolvedBackBets(string Offerid, Bet bet);
        Task<List<Bet>> ManageInvolvedLayBets(string Offerid, Bet bet);


        Task<bool> ZeroBLequdity(string id);
        Task<bool> ZeroLLequdity(string id);

        Task<List<Bet>> ManageInvolvedLowLayBets(string Offerid, Bet bet);
        Task<List<Bet>> ManageInvolvedLowBackBets(string Offerid, Bet bet);


        Task AddNotification(Bet bets, string type, RequestingUser user);

        Task<List<OfferResultDto>> Getoffers(int EventId, string MarketName);
    
         Task<List<Bet>> getunmatched(string UserID);
         Task<List<Bet>> getmatched(string UserID);
    }
}
