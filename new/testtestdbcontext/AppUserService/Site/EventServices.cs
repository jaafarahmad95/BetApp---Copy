using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.DataAccess;
using testtest.dto.Site;
using testtest.Models.Site;

namespace testtest.Service.Site
{
    public class EventServices : IEventServices
    {
        protected IMongoCollection<Event> _Event;
        private readonly IMongoDbContext _context;
        private readonly IMapper _mapper;
      
        public EventServices(IMongoDbContext context,IMapper mapper)
        {
            _context = context;
            _Event = _context.GetCollection<Event>(typeof(Event).Name);
            _mapper = mapper;
        }
        public async Task<List<string>> GetRegion()
        {
            var filter1 = new BsonDocument();
            var filter2 = Builders<Event>.Filter.Ne("Fixture.stage", "Ended");
            var filter = Builders<Event>.Filter.And(filter1, filter2);
            var RegionList = await _Event.Distinct<string>("Fixture.region", filter2).ToListAsync();
             return RegionList;
            
        }

        public async Task<List<string>> GetCompInRegion(string region)

        {
            var filter2 = Builders<Event>.Filter.Eq("Fixture.region", region);
            var filter3 = Builders<Event>.Filter.Ne("Fixture.stage", "Ended");
            var filter = Builders<Event>.Filter.And(filter3, filter2);
            var list = await _Event.Distinct<string>("Fixture.competition", filter).ToListAsync();
            return list;
           
        }

        public async Task<List<Event>> GetEventInComp(string region, string comp)
        {
            var filter3 = Builders<Event>.Filter.Eq("Fixture.region", region);
            var filter4 = Builders<Event>.Filter.Eq("Fixture.competition", comp);
            var filter5 = Builders<Event>.Filter.And(filter3, filter4);
            var listofevents = await _Event.Distinct<int>("Fixture.EventId", filter5).ToListAsync();
            var test = await _Event.Find(x => x.Fixture.EventId == listofevents[0]).FirstOrDefaultAsync();
            List<Event> x = new List<Event> { await _Event.Find(x => x.Fixture.EventId == listofevents[0]).FirstOrDefaultAsync() };
            if (listofevents.Count > 1)
            {
                for (var xx = 1; xx < listofevents.Count; xx++)
                {
                    var z = await _Event.Find(x => x.Fixture.EventId == listofevents[xx]).FirstOrDefaultAsync();
                    x.Add(z);
                }
                return x;
            }
           
            return x;
           
           

        }

        public async Task<List<Event>> GetLiveEvent()
        {
           
            var filter6 = Builders<Event>.Filter.Eq("Fixture.stage", "Live");
             var listofevents = await _Event.Distinct<int>("Fixture.EventId", filter6).ToListAsync();
            List<Event> x = new List<Event> { await _Event.Find(x => x.Fixture.EventId == listofevents[0]).FirstOrDefaultAsync() };
            for (var xx = 1; xx < listofevents.Count; xx++)
            {
                var z = await _Event.Find(x => x.Fixture.EventId == listofevents[xx]).FirstOrDefaultAsync();
                x.Add(z);
            }

            return x;


        }

        public async Task<List<string>> GetEventMarkets(int eventId)
        {
            var filter7 = Builders<Event>.Filter.Eq("Fixture.EventId", eventId);
            var listofmarkets = await _Event.Distinct<string>("Fixture.markets.name.value", filter7).ToListAsync();
            return listofmarkets;
        }

        public async Task<List<Results>> GetMarketsodds(int eventId, string marketName)
        {
             marketName = marketName.Replace("%2F", "/");
            var filter8 = Builders<Event>.Filter.Eq("Fixture.EventId", eventId);
           
            var marketodds = await _Event.Distinct<Market>("Fixture.markets",filter8).ToListAsync();
            marketodds.Sort((a, b) => a.name.value.CompareTo(b.name.value));
         
            List<Results> odds = new List<Results>();
            foreach (var item in marketodds)
            { var x = item.results;
                if (item.name.value == marketName)
                    foreach (var result in x)
                    {
                        odds.Add(result);
                    }
                    
            }
            return odds;
           
        }

        public async Task<ScoreBoared> GetScoreBoared(int eventid)
        {
            var filter11 = Builders<Event>.Filter.Eq("Fixture.EventId", eventid);
            var scoreboard = await _Event.Distinct<ScoreBoared>("Fixture.scoreboard", filter11).FirstOrDefaultAsync();
            return scoreboard;
        }

        public async Task<List<RegComp>> GetRegComp()
        {
            var filter1 = new BsonDocument();
            var RegionList = await _Event.Distinct<string>("Fixture.region", filter1).ToListAsync();
            List<RegComp> regComps = new List<RegComp>();
            foreach (var item in RegionList)
            {
                var filter3 = Builders<Event>.Filter.Eq("Fixture.region", item);
                var list = await _Event.Distinct<string>("Fixture.competition", filter3).ToListAsync();
                RegComp regComp = new RegComp()
                {
                    region = item,
                    competition = list
                    
                
                };
                regComps.Add(regComp);
            }
            regComps.Sort((a,b) => a.region.CompareTo(b.region));
            return regComps;

        }

         public async Task<List<RegComp>> GetLiveRegComp()
        {
            var filter1 = new BsonDocument();
            var filter2 = Builders<Event>.Filter.Eq("Fixture.stage", "Live");
             var filter = Builders<Event>.Filter.And(filter1, filter2);
            var RegionList = await _Event.Distinct<string>("Fixture.region", filter).ToListAsync();
            List<RegComp> regComps = new List<RegComp>();
            foreach (var item in RegionList)
            {
                var filter3 = Builders<Event>.Filter.Eq("Fixture.region", item);
                var filter4 = Builders<Event>.Filter.And(filter2, filter3);
                var list = await _Event.Distinct<string>("Fixture.competition", filter4).ToListAsync();
                RegComp regComp = new RegComp()
                {
                    region = item,
                    competition = list

                };
                regComps.Add(regComp);
            }
            regComps.Sort((a,b) => a.region.CompareTo(b.region));
            return regComps;

        }

        public async Task<List<Event>> Search(string searchtearm)
        {
            var list = await _Event.Find(x =>x .Fixture.teams.Contains(searchtearm)).ToListAsync();
            return list ; 
        }
    }
}
