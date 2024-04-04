using Microsoft.AspNetCore.Authentication;
using MongoDB.Bson;
using MongoDB.Driver;

namespace c_ApiLayout.Utilities
{
    public class Log
    {
        public static void LogEvent(IMongoCollection<BsonDocument> logCollection, string name, string email, string description, string ticketID, DateTime date)
        {
            var logTicket = new BsonDocument
         {
             {"Name", name },
             {"Email", email },
             {"Description", description },
             {"Date Created", date.ToString("yyyy-MM-dd HH:mm:ss")},
             {"ticketID", ticketID }
         };
            logCollection.InsertOne(logTicket);
        }
    }
}
