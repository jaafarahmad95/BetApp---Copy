using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using testtest.Models;

namespace testtest.Models
{
    public class CurrencyUpdate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Key]
        [BsonElement]
        public int CurrenceyId { get; set; }
        [Required]
        [BsonElement]
        public decimal USD { get; set; }
        [Required]
        [BsonElement]
        public decimal STG { get; set; }
        [Required]
        [BsonElement]
        public decimal EURO { get; set; }
        [Required]
        [BsonElement]
        public DateTimeOffset UpdateDate { get; set; } 
        [Required]
        [BsonElement]
        public  string AppuserId { get; set; }

    }
}
