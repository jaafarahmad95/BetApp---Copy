using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace testtestdbcontext.testtest.Models
{
    public class Withdrow
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } 

        [BsonElement]
        public DateTime Date { get; set; } = DateTime.Now;
        [BsonElement]
        public string UserId { get; set; }

        [BsonElement]
        public double Amount { get; set; }

        [BsonElement]
        public string BankAccountNo {get;set;}
    }
}