using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingApi.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "2507a570-32a3-4fea-8f20-d6884b7d48d9",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = "c2b5a30a-9354-4bb9-860a-bb6c6a32c5ee",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new IdentityRole
            {
                Id = "d3f1e8b4-5f4c-4e2e-9f7a-1c2b3a4d5e6f",
                Name = "Hotel Admin",
                NormalizedName = "HOTEL ADMIN"
            }
        );
    }
}