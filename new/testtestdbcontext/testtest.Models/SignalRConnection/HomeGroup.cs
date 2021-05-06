using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace testtest.Models.SignalRConnection
{
    public class HomeGroup
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public string ConnectionId {get;set;}
  
        [BsonElement]
        public string ConnectionGroups { get; set; }
    }
}