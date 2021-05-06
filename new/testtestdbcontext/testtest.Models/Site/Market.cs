using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
    public class Market
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public int marketid { get; set; }
        [BsonElement]
        public Name name { get; set; }
        [BsonElement]
        public IEnumerable<Results> results { get; set; }
        [BsonElement]
        public string visibility { get; set; }
        [BsonElement]
        public bool isMain { get; set; }
    }
}
