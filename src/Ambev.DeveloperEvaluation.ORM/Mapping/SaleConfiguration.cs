using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

/// <summary>
/// EF Core mapping configuration for the Sale Entity.
/// Maps Value Objects using the OwnsOne fluent API.
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.Number).ValueGeneratedOnAdd();

        builder.OwnsOne(s => s.Customer, customer =>
        {
            customer.Property(c => c.Value).HasColumnName("CustomerId");
            customer.Property(c => c.Description).HasColumnName("CustomerName").HasMaxLength(150);
        });

        builder.OwnsOne(s => s.Branch, branch =>
        {
            branch.Property(b => b.Value).HasColumnName("BranchId");
            branch.Property(b => b.Description).HasColumnName("BranchName").HasMaxLength(150);
        });

        builder.OwnsOne(s => s.TotalAmount, money =>
        {
            money.Property(m => m.Value).HasColumnName("TotalAmount")
                .HasColumnType("numeric(18, 2)").IsRequired();
        });

        builder.HasMany(s => s.Items)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}
