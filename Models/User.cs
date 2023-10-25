using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SoccerQuizApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("UserName")]
        public string UserName { get; set; } = null!;

        [BsonElement("Password")]
        public string Password { get; set; } = null!;

        [BsonElement("IsAdmin")]
        public bool IsAdmin { get; set; }
    }
}
