using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SoccerQuizApi.Models;

namespace SoccerQuizApi.Services
{
    public class QuizService
    {
        private readonly IMongoCollection<Quiz> _quizCollection;

        public QuizService(
            IOptions<SoccerQuizDBSettings> quizStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                quizStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                quizStoreDatabaseSettings.Value.DatabaseName);

            _quizCollection = mongoDatabase.GetCollection<Quiz>(
                quizStoreDatabaseSettings.Value.QuizCollectionName);
        }

        public async Task<List<Quiz>> GetAsync() =>
            await _quizCollection.Find(_ => true).ToListAsync();

        public async Task<Quiz?> GetAsync(string id) =>
            await _quizCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Quiz newQuiz) =>
            await _quizCollection.InsertOneAsync(newQuiz);

        public async Task UpdateAsync(string id, Quiz updatedQuiz) =>
            await _quizCollection.ReplaceOneAsync(x => x.Id == id, updatedQuiz);

        public async Task RemoveAsync(string id) =>
            await _quizCollection.DeleteOneAsync(x => x.Id == id);
    }
}
