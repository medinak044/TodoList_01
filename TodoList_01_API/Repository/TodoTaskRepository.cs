using TodoList_01_API.Data;
using TodoList_01_API.Models;
using TodoList_01_API.Repository.IRepository;

namespace TodoList_01_API.Repository;

public class TodoTaskRepository: Repository<TodoTask>, ITodoTaskRepository
{
    private AppDbContext _context;
    public TodoTaskRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
