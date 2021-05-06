using MongoDB.Bson.Serialization.Attributes;

namespace testtest.Models.Site
{
    public class Messages
    {
        [BsonElement]
        public int messageType { get; set; }
        [BsonElement]
        public string content { get; set; }
        [BsonElement]
        public string timer { get; set; }
        [BsonElement]
        public int order { get; set; }
        [BsonElement]
        public string teamType { get; set; }
    }
}