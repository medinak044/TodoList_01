using System.ComponentModel.DataAnnotations;

namespace TodoList_01_API.DTOs;

public class RegistrationReqDto
{
    [Required]
    public string UserName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
