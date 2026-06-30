using FoundationKit.Domain.Models;

namespace InventorySystem.Products.Domain.Models;

public class Product : BaseModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    /// <summary>
    /// Para no alargar la solucion se opto por crear el campo solo category,
    /// lo correcto es crear una tabla y relacionarla 1-N
    /// </summary>
    public required string Category { get; set; }
    public string? Sku { get; set; }
}
