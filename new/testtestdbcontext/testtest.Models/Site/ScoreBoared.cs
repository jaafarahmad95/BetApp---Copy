using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.Site
{
    [BsonIgnoreExtraElements]
    public  class ScoreBoared
    {

        [BsonElement]
        public int periodId { get; set; }
        [BsonElement]
        public string bookieTicker { get; set; }
        [BsonElement]
        public string parentPeriod { get; set; }
        [BsonElement]
        public string source { get; set; }
        [BsonElement]
        public RedCards redCards { get; set; }
        [BsonElement]
        public bool started { get; set; }
        [BsonElement]
        public int scoreboardId { get; set; }
        [BsonElement]
        public string score { get; set; }
       [BsonElement]
        public IEnumerable<Messages> messages { get; set; }
        [BsonElement]
        public string period { get; set; }
        [BsonElement]
        public YellowCards yellowCards { get; set; }
        [BsonElement]
        public Penalties penalties { get; set; }
        [BsonElement("substitutions")]
        public Substitutions substitutions { get; set; }
        [BsonElement]
        public Timer timer { get; set; }
        [BsonElement]
        public PenaltiesControl penaltiesControl { get; set; }
        [BsonElement]
        public PlayerInfo playerInfo { get; set; }
        [BsonElement]
        public ScoreDetailed scoreDetailed { get; set; }
        [BsonElement]
        public int sportId { get; set; }
        [BsonElement]
        public Corners corners { get; set; }





    }
}
