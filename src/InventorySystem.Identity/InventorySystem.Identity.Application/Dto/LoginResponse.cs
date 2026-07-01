namespace InventorySystem.Identity.Application.Dto;

public class LoginResponse
{
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}
