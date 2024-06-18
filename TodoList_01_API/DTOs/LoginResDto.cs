namespace TodoList_01_API.DTOs;

// Provides client with user info + token
// Similar to "AppUserDto"
public class LoginResDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime DateCreated { get; set; } // Field used for helping to convert date value to string
    public string DateCreatedStr { get; set; }
    public string Token { get; set; } // Includes Id, UserName, Email in claims
    public string RefreshToken { get; set; }
}
