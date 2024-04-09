using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

public class TicketDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string TicketID { get; }
    public string TicketStatus { get; set; }
    public TicketDto()
    {
        TicketID = Guid.NewGuid().ToString();
        TicketStatus = "Open";
    }
}

public class AdminDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}