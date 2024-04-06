using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c_ApiLayout.Controllers
{
    public class MongoService
    {
        private readonly IMongoCollection<BsonDocument> _testcollection;

        public MongoService(IMongoDatabase database)
        {
            _testcollection = database.GetCollection<BsonDocument>("Tickets");
        }

        public async Task<List<BsonDocument>> GetDataFromMongo()
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            return await _testcollection.Find(filter).ToListAsync();
        }
    }
}
