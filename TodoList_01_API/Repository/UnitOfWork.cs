using Microsoft.EntityFrameworkCore;
using TodoList_01_API.Data;
using TodoList_01_API.Helpers;
using TodoList_01_API.Repository.IRepository;

namespace TodoList_01_API.Repository;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UnitOfWork(
        AppDbContext context,
        IHttpContextAccessor httpContextAccessor
        )
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    public ITodoListRepository TodoLists => new TodoListRepository(_context);
    public ITodoTaskRepository TodoTasks => new TodoTaskRepository(_context);
    public IWorkspaceRepository Workspaces => new WorkspaceRepository(_context);

    // Used for .NET session (cookie) authentication
    public string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User.GetUserId(); // Gets user Id value from cookie
    }

    public async Task<bool> SaveAsync()
    {
        var saved = await _context.SaveChangesAsync(); // Returns an integer
        return saved > 0 ? true : false;
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
