using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
   public class Participants
    {
        [BsonElement]
        public int first_participantId{ get; set; }

        [BsonElement]
        public int second_participantId { get; set; }
        [BsonElement]
        public string first_name { get; set; }
        [BsonElement]
        public string second_name { get; set; }

    }
}
