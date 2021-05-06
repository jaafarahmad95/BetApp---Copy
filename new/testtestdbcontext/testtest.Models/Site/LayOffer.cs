using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
    public class LayOffer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public  string UserId { get; set; } 
       
        [BsonElement]
        public int EventId { get; set; }
        [BsonElement]
        public string Teams { get; set; }
        [BsonElement]
        public string MarketName { get; set; }
        [BsonElement]
        public string Side { get; set; } = "lay";
        [BsonElement]
        public string RunnerName { get; set; }
        [BsonElement]
        public double Odds { get; set; }
        [BsonElement]
        public bool KeepInPlay { get; set; }

        [BsonElement]
        public string Status { get; set; } = "open";
        [BsonElement]
        public double Liquidity { get; set; }
        [BsonElement]
        public  List<Bet> MatchedBets { get; set; }
        [BsonElement]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        [BsonElement]
        public string visibility { get; set; } = "Visible";

    }
}
