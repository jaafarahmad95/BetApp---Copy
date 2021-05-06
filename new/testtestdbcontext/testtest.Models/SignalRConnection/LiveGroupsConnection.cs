using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace testtest.Models.SignalRConnection
{
    public class LiveGroupConnection
    {
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public string ConnectionId {get;set;}
        
        [BsonElement]
        public List<string> ConnectionGroups { get; set; }
    }
}