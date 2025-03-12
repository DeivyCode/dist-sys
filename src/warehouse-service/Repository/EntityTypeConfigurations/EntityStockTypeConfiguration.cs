using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using warehouse_service.Repository.Entities;

namespace warehouse_service.Repository.EntityTypeConfigurations;

public class EntityStockTypeConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");
        builder.HasKey(s => s.StockId);
        builder.Property(s => s.StockId).ValueGeneratedOnAdd();
        builder.HasOne<Product>(x => x.Product).WithOne().HasForeignKey<Stock>(x => x.ProductId);
        builder.Property(s => s.Price2);
        builder.Property(s => s.Price1);
        builder.Property(s => s.Lote);
        builder.Property(s => s.Quantity).IsRequired();
        builder.Property(s => s.Price).IsRequired();
        builder.Property(s => s.Cost).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();
        builder.Property(s => s.IsDeleted).HasDefaultValue(false).IsRequired();

        //Index
        builder.HasIndex(s => s.ProductId);
    }
}