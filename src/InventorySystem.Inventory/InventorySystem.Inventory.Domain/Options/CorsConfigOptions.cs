namespace InventorySystem.Inventory.Domain.Options;

public class CorsConfigOptions
{
    public const string Cors = "Cors";

    public List<string>? OriginsAllowed { get; set; }
    public List<string>? MethodsAllowed { get; set; }
}
