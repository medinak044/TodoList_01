using Microsoft.AspNetCore.Identity;

namespace TodoList_01_API.Models;

public class AppUser: IdentityUser
{
    // Use email as username
    public DateTime DateCreated { get; set; }
    public ICollection<TodoList>? TodoLists { get; set; }
}
