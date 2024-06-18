namespace TodoList_01_API.Helpers;

// Access secret jwt settings through strong typing (Used in Program.cs)
public class AppSettings_Jwt
{
    public string? Secret { get; set; }
}
