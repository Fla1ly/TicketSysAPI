using c_ApiLayout.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace c_ApiLayout.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class apiLayoutController : ControllerBase
    {
        private readonly IMongoCollection<BsonDocument> _testCollection;
        private readonly IConfiguration _configuration;

        public apiLayoutController(IConfiguration configuration, IMongoClient mongoClient)
        {
            _configuration = configuration;

            var client = mongoClient;
            var userDatabase = client.GetDatabase("TicketSystems");
            _testCollection = userDatabase.GetCollection<BsonDocument>("Tickets");
        }

        [HttpPost("sendTicket")]
        public IActionResult dtoEndpoint([FromBody] UserDto userForm)
        {
            string name = userForm.name;
            string email = userForm.email;
            string description = userForm.description;
            DateTime date = DateTime.Now;
            string ticketID = Guid.NewGuid().ToString();

            Log.LogEvent(_testCollection, email, name, description, ticketID, date);

            return Ok(new { message = "created ticket", email = email, description = description, ticketID = ticketID, date = date,});
        }

        [HttpGet("tickets")]

        public IActionResult GetTicketById(string ticketID)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ticketID);
            var ticket = _testCollection.Find(filter).FirstOrDefault();

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }
    }
}