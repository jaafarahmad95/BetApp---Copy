using MongoDB.Bson.Serialization.Attributes;

namespace testtest.Models.Site
{
    public class Teams
    {
       
        [BsonElement]
        public bool visible { get; set; }
        [BsonElement]
        public string teamName { get; set; }
        [BsonElement]
        public string shirtColor { get; set; }
        [BsonElement]
        public string shortsColor { get; set; }
    }
}