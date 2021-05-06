using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace testtest.Models
{
    public class Currency
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Key]
        [BsonElement]
        public int CurrenceyId { get; set; }
        [Required]
        [StringLength(50)]
        [BsonElement]
        public string Name { get; set; }
        [Required]
        [BsonElement]
        public char Symbol { get; set; }
        [Required]
        [StringLength(10)]
        [BsonElement]
        public string Code { get; set; }
        [Required]
        [BsonElement]
        public decimal Value { get; set; }
        [BsonElement]
        public bool IsDefault { get; set; }
        [BsonElement]
        public DateTime LastUpdate { get; set; }
        [BsonElement]
        public string UserId { get; set; }
    }
}
