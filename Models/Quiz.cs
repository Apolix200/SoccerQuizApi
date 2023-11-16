using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SoccerQuizApi.Models
{
    public class Quiz
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("QuizName")]
        public string? QuizName { get; set; }

        [BsonElement("QuestionAndAnswers")]
        public List<QuestionAndAnswer> QuestionAndAnswers { get; set; } = null!;

        [BsonElement("IsActive")]
        public bool IsActive { get; set; } = false;
    }

    public class QuestionAndAnswer
    {
        [BsonElement("Question")]
        public string Question { get; set; } = "";

        [BsonElement("Answers")]
        public List<string> Answers { get; set; } = new List<string>();

        [BsonElement("CorrectAnswer")]
        public int CorrectAnswer { get; set; }
    }

    public class UserQuizResult
    {
        public Quiz Quiz { get; set; }

        public string? UserId { get; set; }

    }
}
