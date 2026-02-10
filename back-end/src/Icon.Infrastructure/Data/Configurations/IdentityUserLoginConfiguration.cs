using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Icon.Infrastructure.Data.Configurations;

internal sealed class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder) => builder.ToTable("user_logins");
}
