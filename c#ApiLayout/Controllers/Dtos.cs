using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

public class UserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Description { get; set; }
    public string TicketID { get; }

    public UserDto()
    {
        TicketID = Guid.NewGuid().ToString();
    }
}