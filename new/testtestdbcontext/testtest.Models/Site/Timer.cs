using MongoDB.Bson.Serialization.Attributes;
using System;

namespace testtest.Models.Site
{
    public class Timer
    {
        [BsonElement]
        public bool running { get; set; }
        [BsonElement]
        public DateTime basetime { get; set; }
        [BsonElement]
        public bool visible { get; set; }
        [BsonElement]
        public int seconds { get; set; }
    }
}