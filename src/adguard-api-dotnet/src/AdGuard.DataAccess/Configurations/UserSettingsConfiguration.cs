using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdGuard.DataAccess.Configurations;

/// <summary>
/// Entity configuration for <see cref="UserSettingsEntity"/>.
/// </summary>
public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettingsEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserSettingsEntity> builder)
    {
        builder.ToTable("UserSettings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Value)
            .HasMaxLength(4000);

        builder.Property(e => e.ValueType)
            .HasConversion<int>();

        builder.Property(e => e.Category)
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        // Unique constraint on Key
        builder.HasIndex(e => e.Key)
            .IsUnique()
            .HasDatabaseName("UX_UserSettings_Key");

        // Index for category-based queries
        builder.HasIndex(e => e.Category)
            .HasDatabaseName("IX_UserSettings_Category");
    }
}
