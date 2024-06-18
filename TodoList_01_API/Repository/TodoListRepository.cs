using TodoList_01_API.Data;
using TodoList_01_API.Models;
using TodoList_01_API.Repository.IRepository;

namespace TodoList_01_API.Repository;

public class TodoListRepository: Repository<TodoList>, ITodoListRepository
{
    private AppDbContext _context;
    public TodoListRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
