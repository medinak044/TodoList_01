using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoList_01_API.Models;

namespace TodoList_01_API.Data;

public class AppDbContext: IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }

    // AppUser handled by Identity framework
    public DbSet<TodoList> TodoLists { get; set; }
    public DbSet<TodoTask> TodoTasks { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
}
