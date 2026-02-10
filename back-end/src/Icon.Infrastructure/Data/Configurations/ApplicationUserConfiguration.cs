using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Icon.Infrastructure.Identity.Models;

namespace Icon.Infrastructure.Data.Configurations;

internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder) => builder.ToTable("users");
}
