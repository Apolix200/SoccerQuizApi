using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SoccerQuizApi.Models;

namespace SoccerQuizApi.Services
{
    public class ResultService
    {
        private readonly IMongoCollection<Result> _resultCollection;

        public ResultService(
            IOptions<SoccerQuizDBSettings> resultStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                resultStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                resultStoreDatabaseSettings.Value.DatabaseName);

            _resultCollection = mongoDatabase.GetCollection<Result>(
                resultStoreDatabaseSettings.Value.ResultCollectionName);
        }

        public async Task<List<Result>> GetAsync() =>
            await _resultCollection.Find(_ => true).ToListAsync();

        public async Task<Result?> GetAsync(string id) =>
            await _resultCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Result newResult) =>
            await _resultCollection.InsertOneAsync(newResult);

        public async Task UpdateAsync(string id, Result updatedResult) =>
            await _resultCollection.ReplaceOneAsync(x => x.Id == id, updatedResult);

        public async Task RemoveAsync(string id) =>
            await _resultCollection.DeleteOneAsync(x => x.Id == id);

        public async Task RemoveManyByQuizAsync(string id) =>
            await _resultCollection.DeleteManyAsync(x => x.QuizId == id);

        public async Task RemoveManyByUserAsync(string id) =>
            await _resultCollection.DeleteManyAsync(x => x.UserId == id);
    }
}
