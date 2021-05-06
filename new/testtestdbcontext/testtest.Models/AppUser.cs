using AspNetCore.Identity.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace testtest.Models
{
    public class AppUser : MongoIdentityUser 
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }

        [BsonElement("Country")]
        public string Country { get; set; }

        [BsonElement("BirthDate")]
        public DateTime birthdate { get; set; }

        [BsonElement("PersonalID")]
        public string PersonalID { get; set; }

        [BsonElement("City")]
        public string City { get; set; }

        [BsonElement("Currency")]
        public string Currency { get; set; } = "TL";


    }
}
