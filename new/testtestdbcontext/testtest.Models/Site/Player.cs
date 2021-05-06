using MongoDB.Bson.Serialization.Attributes;
using System;

namespace testtest.Models.Site
{
    public class Player
    {
        [BsonElement]
        public int one { get; set; }
        [BsonElement]
        public int two { get; set; }
        [BsonElement]
        public int three { get; set; }
        [BsonElement]
        public int four { get; set; }
        [BsonElement]
        public int five { get; set; }

        [BsonElement]
        public int six { get; set; }
        [BsonElement]
        public int seven { get; set; }

    }
}