using FoundationKit.Domain.Config;
using InventorySystem.Products.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem.Products.Infrastructure.EF.Configurations;

public class ProductEfConfiguration : EFCoreConfiguration<Product>
{
    public override void ConfigureEF(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(200);

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(x => x.Sku)
            .IsUnique();

        builder.HasIndex(x => x.Category);
    }
}
