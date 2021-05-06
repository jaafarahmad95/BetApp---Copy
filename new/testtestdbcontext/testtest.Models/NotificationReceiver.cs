using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace testtest.Models
{
    public class NotificationReceiver
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string NotificationId { get; set; }
        public string ReceiverId { get; set; }
        public bool IsDeleted { get; set; }


    }
}
