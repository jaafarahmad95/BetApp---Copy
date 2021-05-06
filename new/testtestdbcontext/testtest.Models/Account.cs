using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models
{
   public class Account
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

        [BsonElement("Commission_Type")]
        public string Commission_Type { get; set; }

        [BsonElement("Currency")]
        public string Currency { get; set; }
        
        [BsonElement("Country")]
        public string Country { get; set; }

        [BsonElement("Balance")]
        public double Balance { get; set; }
        [BsonElement("Commission_resreve")]
        public double Commission_resreve { get; set; }

        [BsonElement("Free_founds")]
        public double Free_founds { get; set; }

        [BsonElement("Language")]
        public string Language { get; set; }

        [BsonElement("Exchange_type")]
        public string Exchange_type { get; set; }
        [BsonElement("Odds_type")]
        public string Odds_type { get; set; }
        [BsonElement("Show_bet_Confirm")]
        public bool Show_bet_Confirm { get; set; }

        [BsonElement("Bet_Slip_pinned")]
        public bool Bet_Slip_pinned { get; set; }

        [BsonElement("Roles")]
        public string Roles { get; set; }
        [BsonElement("BirthDate")]
        public DateTime birthdate { get; set; }

        [BsonElement("PersonalID")]
        public string PersonalID { get; set; }

        [BsonElement("City")]
        public string City { get; set; }

       


    }
}
