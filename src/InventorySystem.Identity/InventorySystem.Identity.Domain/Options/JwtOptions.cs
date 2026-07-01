namespace InventorySystem.Identity.Domain.Options;

public class JwtOptions
{
    public const string Jwt = "Jwt";

    public required string Key { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpirationMinutes { get; set; } = 60;
}
