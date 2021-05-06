using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace testtestdbcontext.testtest.Models.Site
{
    public class BetCounter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement]
        public string EventId { get; set; }

        [BsonElement]
        public int betcounter{get;set;}
        
    }
}