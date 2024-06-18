using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList_01_API.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public string JwtId { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime AddedDate { get; set; } //= DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }


    [ForeignKey(nameof(AppUser))]
    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}
