using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
  public  class Sport
    {
        [BsonElement("Name")]
        public string Name;
      
    }
}
