namespace InventorySystem.Identity.Domain.Options;

public class SeedOptions
{
    public const string Seed = "Seed";

    public required SeedUserOptions AdminUser { get; set; }
    public required SeedUserOptions NormalUser { get; set; }
}

public class SeedUserOptions
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
