using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyEmployees.Api.Data.SeedData;
public class UserSeed : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new IdentityRole { Name = "Manager", NormalizedName = "MANAGER" });
        builder.HasData(new IdentityRole { Name = "Administrator", NormalizedName = "ADMINISTRATOR" });
    }
}
