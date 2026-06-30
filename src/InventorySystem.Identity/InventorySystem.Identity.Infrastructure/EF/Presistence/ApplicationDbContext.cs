using FoundationKit.Domain.Persistence;
using InventorySystem.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Identity.Infrastructure.EF.Presistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : FoundationKitIdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
