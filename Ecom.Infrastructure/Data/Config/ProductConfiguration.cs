using Ecom.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name).IsRequired()
                .HasMaxLength(50);
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Price).IsRequired()
                .HasPrecision(18, 2);
             builder.Property(x => x.StockQuantity).IsRequired();
             //builder.HasOne(x => x.Category)
             //   .WithMany(c => c.Products)
             //   .HasForeignKey(x => x.CategoryId)
             //   .OnDelete(DeleteBehavior.Cascade);
             // builder.HasMany(x => x.Photos)
             //   .WithOne(p => p.Product)
             //   .HasForeignKey(p => p.ProductId)
             //   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new Product { Id = 1, Name = "Smartphone", Description = "Latest model smartphone with advanced features", Price = 699.99m, StockQuantity = 50, CategoryId = 1 },
                new Product { Id = 2, Name = "Laptop", Description = "High-performance laptop for work and gaming", Price = 1299.99m, StockQuantity = 30, CategoryId = 1 }
                );
        }
    }
}
