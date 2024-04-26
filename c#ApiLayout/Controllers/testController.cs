using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace c_ApiLayout.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class apiLayoutController : ControllerBase
    {
        private readonly IMongoCollection<BsonDocument> _testCollection;
        private readonly IMongoCollection<BsonDocument> _userCollection;
        private readonly ILogger<apiLayoutController> _logger;
        private readonly IConfiguration _configuration;

        public apiLayoutController(IConfiguration configuration, IMongoClient mongoClient, ILogger<apiLayoutController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _testCollection = mongoClient.GetDatabase("TicketSystems").GetCollection<BsonDocument>("Tickets");
            _userCollection = mongoClient.GetDatabase("TicketSystems").GetCollection<BsonDocument>("Users");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminDto adminDto)
        {
            if (IsValidAdmin(adminDto.Username, adminDto.Password))
            {
                return Ok(new { message = "Login successful" });
            }
            else
            {
                return Unauthorized();
            }
        }
        private bool IsValidAdmin(string username, string password)
        {
            var user = _userCollection.Find(Builders<BsonDocument>.Filter.Eq("username", username) & Builders<BsonDocument>.Filter.Eq("password", password)).FirstOrDefault();
            return user != null;
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
                { "Subject", ticketDto.Subject },
                { "Description", ticketDto.Description },
                { "Date Created", DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm")},
                { "ticketID", ticketDto.TicketID },
                { "TicketStatus", ticketDto.TicketStatus = "Open" }
            };

            _testCollection.InsertOne(ticketDocument);

            _logger.LogInformation("New ticket created. Ticket ID: {TicketID}, Name: {Name}, Email: {Email}, Subject: {Subject}, Description: {Description}", ticketDto.TicketID, ticketDto.Name, ticketDto.Email, ticketDto.Subject, ticketDto.Description);

            return Ok(new { message = "created ticket", name = ticketDto.Name, email = ticketDto.Email, subject = ticketDto.Subject, description = ticketDto.Description, ticketID = ticketDto.TicketID, date = DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm"), ticketStatus = ticketDto.TicketStatus });
        }

        [HttpPost("Register")]

        public IActionResult CreateUser([FromBody] AdminDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDocument = new BsonDocument
            {
                { "username", userDto.Username },
                { "password", userDto.Password },
            };

            _userCollection.InsertOne(userDocument);

            _logger.LogInformation("New admin created. username: {username}, password: {password}", userDto.Username, userDto.Password);

            return Ok(new { message = "created admin", username = userDto.Username, password = userDto.Password});
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
                    Subject = ticket.GetValue("Subject").AsString,
                    Description = ticket.GetValue("Description").AsString,
                    DateCreated = DateTime.Parse(ticket.GetValue("Date Created").AsString),
                    TicketStatus = ticket.GetValue("TicketStatus").AsString,
                    TicketID = ticket.GetValue("ticketID").AsString
                });
            }
            return Ok(ticketList);
        }

        [HttpGet("ticket/{ticketId}")]
        public IActionResult GetTicket(string ticketId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("ticketID", ticketId);
            var ticket = _testCollection.Find(filter).FirstOrDefault();

            if (ticket == null)
            {
                _logger.LogInformation("Ticket not found with ID: {TicketID}", ticketId);
                return NotFound("Ticket Not Found");
            }

            var ticketDetails = new
            {
                Name = ticket.GetValue("Name").AsString,
                Email = ticket.GetValue("Email").AsString,
                Subject = ticket.GetValue("Subject").AsString,
                Description = ticket.GetValue("Description").AsString,
                DateCreated = DateTime.Parse(ticket.GetValue("Date Created").AsString).ToString("MM-dd-yyyy HH:mm"),
                TicketStatus = ticket.GetValue("TicketStatus").AsString,
                TicketID = ticket.GetValue("ticketID").AsString
            };

            _logger.LogInformation("Ticket found with ID: {TicketID}", ticketId);

            return Ok(ticketDetails);
        }

        [HttpPost("updateTicketAndAddReply/{ticketId}")]
        public IActionResult UpdateTicketAndAddReply(string ticketId, [FromBody] TicketDto ticketDto)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("ticketID", ticketId);
            var update = Builders<BsonDocument>.Update
                .Set("TicketStatus", ticketDto.NewStatus)
                .Push("Replies", ticketDto.Reply);
            var result = _testCollection.UpdateOne(filter, update);

            if (result.ModifiedCount == 0)
            {
                _logger.LogInformation("Ticket not found with ID: {TicketID}", ticketId);
                return NotFound("Ticket Not Found");
            }

            _logger.LogInformation("Ticket status updated and reply added. Ticket ID: {TicketID}, New Status: {NewStatus}, Reply: {Reply}", ticketId, ticketDto.NewStatus, ticketDto.Reply);

            return Ok(new { message = "Ticket status updated and reply added successfully", ticketId, newStatus = ticketDto.NewStatus, reply = ticketDto.Reply });
        }

        [HttpPost("deleteTicket/{ticketId}")]
        public IActionResult DeleteTicket(string ticketId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("ticketID", ticketId);
            var result = _testCollection.DeleteOne(filter);

            if (result.DeletedCount == 0)
            {
                _logger.LogInformation("Ticket not found with ID: {TicketID}", ticketId);
                return NotFound("Ticket Not Found");
            }

            _logger.LogInformation("Ticket deleted successfully. Ticket ID: {TicketID}", ticketId);

            return Ok(new { message = "Ticket deleted successfully", ticketId });
        }

    }
}
