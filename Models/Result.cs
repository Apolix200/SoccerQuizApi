using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SoccerQuizApi.Models
{
    public class Result
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("QuizId")]
        public string? QuizId { get; set; }

        [BsonElement("UserId")]
        public string? UserId { get; set; }

        [BsonElement("QuizName")]
        public string? QuizName { get; set; }

        [BsonElement("UserName")]
        public string? UserName { get; set; }

        [BsonElement("Answers")]
        public List<int> Answers { get; set; } = null!;

        [BsonElement("Score")]
        public int Score { get; set; }

        [BsonElement("Created")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Created { get; set; }
    }

}
