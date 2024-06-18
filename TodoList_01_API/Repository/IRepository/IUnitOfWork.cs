namespace TodoList_01_API.Repository.IRepository;

public interface IUnitOfWork
{
    ITodoListRepository TodoLists { get; }
    ITodoTaskRepository TodoTasks { get; }
    IWorkspaceRepository Workspaces { get; }
    string GetCurrentUserId();
    Task<bool> SaveAsync();
}
