using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
   public class InvolvedLayBets

    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public string BetId { get; set; }
        [BsonElement]
        public string Status { get; set; } = "open";

        [BsonElement]
        public double RemainingStake { set; get; }
        [BsonElement]
        public string OfferId { set; get; }
        [BsonElement]
        public string UserId { get; set; }
        [BsonElement]
        public string MarketName { get; set; }

    }
}
