using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace testtest.Models
{
    public class SBoardGroups
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