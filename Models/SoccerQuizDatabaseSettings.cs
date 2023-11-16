namespace SoccerQuizApi.Models
{
    public class SoccerQuizDBSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string UsersCollectionName { get; set; } = null!;

        public string QuizCollectionName { get; set; } = null!;

        public string ResultCollectionName { get; set; } = null!;
    }
}
