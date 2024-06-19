using System.ComponentModel.DataAnnotations.Schema;
using TodoList_01_API.Models;

namespace TodoList_01_API.DTOs;

public class TodoListReqDto
{
    public string Title { get; set; }
    public string OwnerId { get; set; }
}
