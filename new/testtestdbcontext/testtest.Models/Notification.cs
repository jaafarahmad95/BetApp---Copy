using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace testtest.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement]
       public string UserId { get; set; }
        [BsonElement]
        public string Title { get; set; }
        [BsonElement]
        public string Type { get; set; }
        [BsonElement]
        public string Details { get; set; }
        [BsonElement]
        public string UserName { get; set; }
        [BsonElement]
        public DateTime Date { get; set; }
        
        
        [BsonElement]
        public string Amount { get; set; }
        [BsonElement]
        public virtual ICollection<NotificationReceiver> NotificationReceivers { get; set; }
    }
}
