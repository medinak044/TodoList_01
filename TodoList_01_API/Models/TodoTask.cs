using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TodoList_01_API.Models;

public class TodoTask
{
    [Key]
    public string Id { get; set; }
    public string Title { get; set; }
    public DateTime DateCreated { get; set; }
    public string? Note { get; set; } // Additional information about the task


    [ForeignKey(nameof(TodoList))]
    public string TodoListId { get; set; }
    //public TodoList TodoList { get; set; }
}
