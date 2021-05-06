using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
    public class Results
    {
     
        [BsonElement]
        public double odds { get; set; }
        [BsonElement]
        public Name name { get; set; }
        [BsonElement]
        public string totalsPrefix { get; set; }

        [BsonElement]
        public SourceName SourceName { get; set; }
        [BsonElement]
        public string visibility { get; set; }
        [BsonElement]
        public int numerator { get; set; }
        [BsonElement]
        public int denominator { get; set; }
        [BsonElement]
        public int americanOdds { get; set; }


    }
}
