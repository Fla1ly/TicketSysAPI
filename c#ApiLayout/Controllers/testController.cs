using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace c_ApiLayout.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class apiLayoutController : ControllerBase
    {
        private readonly IMongoCollection<BsonDocument> _testCollection;
        private readonly ILogger<apiLayoutController> _logger;

        public apiLayoutController(IConfiguration configuration, IMongoClient mongoClient, ILogger<apiLayoutController> logger)
        {
            _logger = logger;
            _testCollection = mongoClient.GetDatabase("TicketSystems").GetCollection<BsonDocument>("Tickets");
        }

        [HttpPost("sendTicket")]
        public IActionResult CreateTicket([FromBody] TicketDto ticketDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticketDocument = new BsonDocument
            {
                { "Name", ticketDto.Name },
                { "Email", ticketDto.Email },
                { "Description", ticketDto.Description },
                { "Date Created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                { "ticketID", ticketDto.TicketID },
                { "TicketStatus", ticketDto.TicketStatus = "Open" }
            };

            _testCollection.InsertOne(ticketDocument);

            _logger.LogInformation("New ticket created. Ticket ID: {TicketID}, Name: {Name}, Email: {Email}, Description: {Description}", ticketDto.TicketID, ticketDto.Name, ticketDto.Email, ticketDto.Description);

            return Ok(new { message = "created ticket", email = ticketDto.Email, description = ticketDto.Description, ticketID = ticketDto.TicketID, date = DateTime.UtcNow, ticketStatus = ticketDto.TicketStatus });
        }

        [HttpGet("tickets")]
        public IActionResult GetTickets()
        {
            var tickets = _testCollection.Find(new BsonDocument()).ToList();

            _logger.LogInformation("Tickets retrieved.");

            var ticketList = new List<object>();
            foreach (var ticket in tickets)
            {
                ticketList.Add(new
                {
                    Name = ticket.GetValue("Name").AsString,
                    Email = ticket.GetValue("Email").AsString,
                    Description = ticket.GetValue("Description").AsString,
                    DateCreated = DateTime.Parse(ticket.GetValue("Date Created").AsString),
                    TicketStatus = ticket.GetValue("TicketStatus").AsString,
                    TicketID = ticket.GetValue("ticketID").AsString
                });
            }
            return Ok(ticketList);
        }
    }
}
