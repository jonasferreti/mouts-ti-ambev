using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

/// <summary>
/// EF Core mapping configuration for the SaleItem Entity.
/// Maps multiple Value Objects related to pricing and quantity.
/// </summary>
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(i => i.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");


        builder.OwnsOne(i => i.Product, product =>
        {
            product.Property(p => p.Value).HasColumnName("ProductId");
            product.Property(p => p.Description).HasColumnName("ProductName").HasMaxLength(150);
        });

        builder.OwnsOne(i => i.Quantity, quantity =>
        {
            quantity.Property(q => q.Value).HasColumnName("Quantity").IsRequired();
        });

        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Value).HasColumnName("UnitPrice").HasColumnType("numeric(18, 2)").IsRequired();
        });

        builder.OwnsOne(i => i.TotalAmount, money =>
        {
            money.Property(m => m.Value).HasColumnName("TotalAmount").HasColumnType("numeric(18, 2)").IsRequired();
        });
    }
}

