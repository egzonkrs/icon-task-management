using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Icon.Domain.Entities;
using Icon.Infrastructure.Data.Converters;

namespace Icon.Infrastructure.Data.Configurations;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(new UlidToStringConverter())
            .HasMaxLength(26)
            .IsRequired();

        builder.Property(t => t.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.OwnsOne(t => t.Title, titleBuilder =>
        {
            titleBuilder.Property(v => v.Value)
                .HasColumnName("Title")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.OwnsOne(t => t.Description, descBuilder =>
        {
            descBuilder.Property(v => v.Value)
                .HasColumnName("Description")
                .HasMaxLength(2000);
        });

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.DueDate);
        builder.Property(t => t.SortOrder).IsRequired().HasDefaultValue(0);
        builder.Property(t => t.IsCompleted).IsRequired();
        builder.Property(t => t.CompletedAt);
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt);

        builder.Ignore(t => t.DomainEvents);

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.IsCompleted);
    }
}
