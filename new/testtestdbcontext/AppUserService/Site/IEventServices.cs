using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.Models.Site;

namespace testtest.Service.Site
{
   public interface IEventServices
    {
        Task<List<string>> GetRegion();
        Task<List<string>> GetCompInRegion(string region);

        Task<List<Event>> GetEventInComp(string region, string comp);

        Task<List<Event>> GetLiveEvent();

        Task<List<string>> GetEventMarkets(int eventid);
       Task<List<Results>> GetMarketsodds(int eventid,string MarketName);

        Task<ScoreBoared> GetScoreBoared(int eventid);
        Task<List<RegComp>> GetRegComp();
        Task<List<RegComp>> GetLiveRegComp();
        Task<List<Event>> Search (string searchtearm);
    }
}
