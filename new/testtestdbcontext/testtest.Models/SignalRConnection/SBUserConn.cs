using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace testtest.Models.SignalRConnection
{
    public class SBUserConn
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public string Username { get; set; }
        [BsonElement]
        public List<string> ConnId { get; set; }
    }
}