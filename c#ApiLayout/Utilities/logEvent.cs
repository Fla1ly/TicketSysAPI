using Microsoft.AspNetCore.Authentication;
using MongoDB.Bson;
using MongoDB.Driver;

namespace c_ApiLayout.Utilities
{
    public class Log
    {
        public static void LogEvent(IMongoCollection<BsonDocument> logCollection, string name, string email, string description, int ticketNum)
        {
            var log = new BsonDocument
         {
             { "name", name },
             { "email", email },
             { "description", description },
             {"ticket", ticketNum }
         };
            logCollection.InsertOne(log);
        }
    }
}
