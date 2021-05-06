using MongoDB.Bson.Serialization.Attributes;

namespace testtest.Models.Site
{
    public class Substitutions 
    {
        [BsonElement]
        public Player player1 { get; set; }
        [BsonElement]
        public Player player2 { get; set; }
    }
}