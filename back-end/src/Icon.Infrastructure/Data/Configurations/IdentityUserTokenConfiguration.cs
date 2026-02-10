using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Icon.Infrastructure.Data.Configurations;

internal sealed class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder) => builder.ToTable("user_tokens");
}
