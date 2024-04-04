using c_ApiLayout.Utilities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using System.Text.Json;
using MongoDB.Bson.Serialization;

namespace c_ApiLayout.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class apiLayoutController : ControllerBase
    {
        private readonly IMongoCollection<BsonDocument> _testCollection;
        private readonly IConfiguration _configuration;
        private readonly IMongoClient _mongoClient;
        private readonly MongoService _mongoService;
        private readonly ILogger<apiLayoutController> _logger;

        public apiLayoutController(IConfiguration configuration, IMongoClient mongoClient, ILogger<apiLayoutController> logger)
        {
            _mongoClient = mongoClient;
            _configuration = configuration;
            _logger = logger;

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
        public IActionResult GetTickets()
        {
            var tickets = _testCollection.Find(new BsonDocument()).ToList();

            _logger.LogInformation("Tickets:");
            var ticketDtos = new List<UserDto>();

            foreach (var ticket in tickets)
            {
                _logger.LogInformation(ticket.ToString());

                var userDto = new UserDto
                {
                    name = ticket.GetValue("Name").AsString,
                    email = ticket.GetValue("Email").AsString,
                    description = ticket.GetValue("Description").AsString,
                };

                ticketDtos.Add(userDto);
            }

            return Ok(ticketDtos);
        }
    }
}