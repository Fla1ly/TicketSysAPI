using c_ApiLayout.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

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
            int ticketNum = 1;
            Log.LogEvent(_testCollection, email, name, description, ticketNum);
            return Ok(email);
        }
    }
}