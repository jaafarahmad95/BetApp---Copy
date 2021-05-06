using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace testtest.Models.Deposit

{
   public class Deposit
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public int MethodId { get; set; }
        [BsonElement]
        public DateTime Date { get; set; } = DateTime.Now;
        [BsonElement]
        public int? CurrencyId { get; set; }
        [BsonElement]
        public string UserId { get; set; }
        [BsonElement]
        public decimal CurrencyRate { get; set; }
        [BsonElement]
        public double Amount { get; set; }
        [BsonElement]
        public virtual DepositMethod depositMethod { get; set; }
        [BsonElement]
        public virtual Currency Currency { get; set; }

    }
}
