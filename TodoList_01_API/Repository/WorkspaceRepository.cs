using TodoList_01_API.Data;
using TodoList_01_API.Models;
using TodoList_01_API.Repository.IRepository;

namespace TodoList_01_API.Repository;

public class WorkspaceRepository: Repository<Workspace>, IWorkspaceRepository
{
    private AppDbContext _context;
    public WorkspaceRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
