namespace InventorySystem.Common.Utilities.Responses;

public abstract class ErrorMessage
{
    public required string Code { get; set; }
    public required string Message { get; set; }
}
