namespace InventorySystem.Common.Utilities.Repositories.Seeds;

public interface ISeed
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
