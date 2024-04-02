using Microsoft.AspNetCore.Mvc.ModelBinding;
public class UserDto
{
    [BindNever]
    public string name { get; set; }
    public string email { get; set; }
    public string description { get; set; }
}