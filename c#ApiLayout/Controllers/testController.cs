using c_ApiLayout.Utilities;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
=======
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using System.Text.Json;
using MongoDB.Bson.Serialization;
>>>>>>> 628e2392f3522d824baf16e28f77867ce601e0e6

namespace c_ApiLayout.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class apiLayoutController : ControllerBase
    {
        private readonly IMongoCollection<BsonDocument> _testCollection;
<<<<<<< HEAD
=======
        private readonly IConfiguration _configuration;
        private readonly IMongoClient _mongoClient;
        private readonly MongoService _mongoService;
>>>>>>> 628e2392f3522d824baf16e28f77867ce601e0e6
        private readonly ILogger<apiLayoutController> _logger;

        public apiLayoutController(IConfiguration configuration, IMongoClient mongoClient, ILogger<apiLayoutController> logger)
        {
<<<<<<< HEAD
            _logger = logger;
            _testCollection = mongoClient.GetDatabase("TicketSystems").GetCollection<BsonDocument>("Tickets");
=======
            _mongoClient = mongoClient;
            _configuration = configuration;
            _logger = logger;

            var client = mongoClient;
            var userDatabase = client.GetDatabase("TicketSystems");
            _testCollection = userDatabase.GetCollection<BsonDocument>("Tickets");
>>>>>>> 628e2392f3522d824baf16e28f77867ce601e0e6
        }



        [HttpPost("sendTicket")]
        public IActionResult CreateTicket([FromBody] UserDto userForm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticketDocument = new BsonDocument
            {
                { "Name", userForm.Name },
                { "Email", userForm.Email },
                { "Description", userForm.Description },
                { "Date Created", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }, // Store as string in ISO format
                { "ticketID", userForm.TicketID }
            };

            _testCollection.InsertOne(ticketDocument);

            _logger.LogInformation("New ticket created. Ticket ID: {TicketID}, Name: {Name}, Email: {Email}, Description: {Description}", userForm.TicketID, userForm.Name, userForm.Email, userForm.Description);

            return Ok(new { message = "created ticket", email = userForm.Email, description = userForm.Description, ticketID = userForm.TicketID, date = DateTime.UtcNow });
        }

        [HttpGet("tickets")]
        public IActionResult GetTickets()
        {
            var tickets = _testCollection.Find(new BsonDocument()).ToList();

<<<<<<< HEAD
            _logger.LogInformation("Tickets retrieved.");

            var ticketList = new List<object>();
            foreach (var ticket in tickets)
            {
                ticketList.Add(new
                {
                    Name = ticket.GetValue("Name").AsString,
                    Email = ticket.GetValue("Email").AsString,
                    Description = ticket.GetValue("Description").AsString,
                    DateCreated = DateTime.Parse(ticket.GetValue("Date Created").AsString).ToUniversalTime(),
                    TicketID = ticket.GetValue("ticketID").AsString
                });
            }

            return Ok(ticketList);
=======
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
>>>>>>> 628e2392f3522d824baf16e28f77867ce601e0e6
        }
    }
}