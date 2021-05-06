using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using testtest.Models.Site;

namespace testtestdbcontext.testtest.Models.Site
{
    public class Ended
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        
        [BsonElement]
        public int EventId { get; set; }

        [BsonElement]
        public Participants participants { get; set; }

        
        [BsonElement]
        public string stage { get; set; }
        
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