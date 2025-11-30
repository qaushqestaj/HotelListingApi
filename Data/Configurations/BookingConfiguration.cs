using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingApi.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(q => q.Status)
                .HasConversion<string>()
                .HasMaxLength(20);


            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.HotelId);
            builder.HasIndex(x => new { x.CheckIn, x.CheckOut });
        }
    }
}