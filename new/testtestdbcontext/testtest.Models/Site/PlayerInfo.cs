using MongoDB.Bson.Serialization.Attributes;

namespace testtest.Models.Site
{
    public class PlayerInfo
    {
       

        [BsonElement]
        public bool colorsAvailable { get; set; }
        [BsonElement]
        public Teams one { get; set; }
        [BsonElement]
        public Teams two { get; set; }
    }
}