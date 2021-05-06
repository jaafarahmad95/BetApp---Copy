using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace testtest.Models.Site
{
    public class RedCards
    {

        [BsonElement]
        public Player player1 { get; set; }
        [BsonElement]
        public Player player2 { get; set; }
    }
}