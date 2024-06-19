using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList_01_API.Models;

public class TodoList
{
    [Key]
    public string Id { get; set; }
    public string Title { get; set; }
    public DateTime DateCreated { get; set; }


    [ForeignKey(nameof(AppUser))]
    public string OwnerId { get; set; }
    public AppUser? Owner { get; set; }
    public ICollection<TodoTask>? TodoTasks { get; set; }
}
