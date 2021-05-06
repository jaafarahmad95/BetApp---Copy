using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
   public class Fixture
    {
     
        [BsonElement]
        public IEnumerable<Market> markets { get; set; }

        [BsonElement]
        public Participants participants { get; set; }

        [BsonElement]
        public int EventId { get; set; }

        [BsonElement]
        public Name name { get; set; }
        [BsonElement]
        public string fixtureType { get; set; }
        [BsonElement]
        public string stage { get; set; }
        [BsonElement]
        public bool liveAlert { get; set; }
        [BsonElement]
        public DateTime startDate { get; set; }
        [BsonElement]
        public DateTime cutOffDate { get; set; }
        [BsonElement]
        public string competition { get; set; }
        [BsonElement]
        public string region { get; set; }
        [BsonElement]
        public string viewType { get; set; }

        [BsonElement]
        public bool isOpenForBetting { get; set; }
        [BsonElement]
        public ScoreBoared scoreboard { get; set; }
        [BsonElement]
        public string teams { get; set; }
        [BsonElement]
        public int first_team_goals{get;set;}
        
        [BsonElement]
        public int second_team_goals{get;set;}
        
        [BsonElement]
        public int Goals_Sum{get;set;}
       
       [BsonElement]
        public string FinalResult{get;set;}
       
       [BsonElement]
       public bool Bet_checked { get; set; }

      
    }
}
