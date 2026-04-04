using Ecom.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecom.Infrastructure.Data.Config
{
    public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.HasData(
                new Photo { Id = 1, ImageName = "https://example.com/photo1.jpg", ProductId = 1 },
                new Photo { Id = 2, ImageName = "https://example.com/photo2.jpg", ProductId = 1 },
                new Photo { Id = 3, ImageName = "https://example.com/photo3.jpg", ProductId = 2 }
                );
        }
    }
}
