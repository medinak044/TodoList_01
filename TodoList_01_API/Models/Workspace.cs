using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList_01_API.Models;

// Only one workspace per user
public class Workspace
{
    [Key]
    public string Id { get; set; }


    [ForeignKey(nameof(AppUser))]
    public string OwnerId { get; set; }
    public AppUser? Owner { get; set; }
    public ICollection<TodoList>? TodoLists { get; set; }
}
