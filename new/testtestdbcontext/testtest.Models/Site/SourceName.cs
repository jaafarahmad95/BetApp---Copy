using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
    public class SourceName
    {
  


        [BsonElement]
        public string value { get; set; }


        [BsonElement]
        public string sign { get; set; }
        
    }
}
