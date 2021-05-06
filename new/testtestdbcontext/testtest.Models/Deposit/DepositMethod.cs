using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Deposit

{
   public class DepositMethod
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement]
        public int MethodId { get; set; }
        [BsonElement]
        public string Name { get; set; }
    }
}
