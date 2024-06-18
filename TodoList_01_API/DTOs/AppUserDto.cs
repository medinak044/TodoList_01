namespace TodoList_01_API.DTOs;

public class AppUserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime DateCreated { get; set; } // Field used for helping to convert date value to string
    public string DateCreatedStr { get; set; }
}
