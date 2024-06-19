using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoList_01_API.DTOs;
using TodoList_01_API.Helpers;
using TodoList_01_API.Models;
using TodoList_01_API.Repository.IRepository;

namespace TodoList_01_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoListController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public TodoListController(
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager
        )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    [HttpGet("GetAllTodoLists")]
    public async Task<ActionResult> GetAllTodoLists()
    {
        var result = await _unitOfWork.TodoLists.GetAllAsync();
        return Ok(result);
    }

    // A single default TodoList should always be assigned to a newly created user
    [HttpPost(nameof(CreateTodoList))]
    public async Task<ActionResult> CreateTodoList(TodoListReqDto todoListForm)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        TodoList todoList = new TodoList { 
            Id = Guid.NewGuid().ToString(),
            Title = todoListForm.Title,
            DateCreated = DateTime.UtcNow,
            OwnerId = todoListForm.OwnerId,
        };

        await _unitOfWork.TodoLists.AddAsync(todoList);
        if (!await _unitOfWork.SaveAsync())
        {
            return BadRequest(new AuthResult()
            {
                Success = false,
                Messages = new List<string>() { "Something went wrong while saving" }
            });
        }

        return Ok(todoList);
    }


}
