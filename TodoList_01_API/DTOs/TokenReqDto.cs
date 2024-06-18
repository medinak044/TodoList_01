using System.ComponentModel.DataAnnotations;

namespace TodoList_01_API.DTOs;

public class TokenReqDto
{
    [Required]
    public string Token { get; set; }
    [Required]
    public string RefreshToken { get; set; }
}
