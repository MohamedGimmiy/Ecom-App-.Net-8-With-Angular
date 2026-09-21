using Ecom.Core.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            builder.OwnsOne(x => x.shippingAddress, n => { n.WithOwner(); });
            builder.HasMany(x=>x.orderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.status).HasConversion(
                s => s.ToString(),
                s => (Status)Enum.Parse(typeof(Status), s));

            builder.Property(m => m.SubTotal).HasColumnType("decimal(18,2)");
        }
    }
}
