using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
    public class Bet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public string UserId { get; set; }
        [BsonElement]
        public string Teams { get; set; }
        [BsonElement]
        public string Side { get; set; }
        [BsonElement]
        public string RunnerName { get; set; }

        [BsonElement]
        public double InStake { get; set; }

        [BsonElement]
        public double RemainingStake { get; set; }

        [BsonElement]
        public double Odds { get; set; }

        [BsonElement]
        public string EventId { get; set; }
        [BsonElement]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [BsonElement]

        public string Status { get; set; }

        [BsonElement]
        public string MarketName { get; set; }
        [BsonElement]
        public double Lalaibelty { get; set; }
        [BsonElement]
        public double Profit { get; set; }


    }
}
