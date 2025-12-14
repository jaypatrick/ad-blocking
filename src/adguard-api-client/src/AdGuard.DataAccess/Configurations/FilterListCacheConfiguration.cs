using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdGuard.DataAccess.Configurations;

/// <summary>
/// Entity configuration for <see cref="FilterListCacheEntity"/>.
/// </summary>
public class FilterListCacheConfiguration : IEntityTypeConfiguration<FilterListCacheEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FilterListCacheEntity> builder)
    {
        builder.ToTable("FilterListCache");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.FilterListId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Url)
            .HasMaxLength(2000);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.Homepage)
            .HasMaxLength(500);

        builder.Property(e => e.DnsServerId)
            .HasMaxLength(100);

        builder.Property(e => e.ETag)
            .HasMaxLength(200);

        // Unique constraint on FilterListId
        builder.HasIndex(e => e.FilterListId)
            .IsUnique()
            .HasDatabaseName("UX_FilterListCache_FilterListId");

        // Indexes for common queries
        builder.HasIndex(e => e.IsEnabled)
            .HasDatabaseName("IX_FilterListCache_IsEnabled");

        builder.HasIndex(e => e.DnsServerId)
            .HasDatabaseName("IX_FilterListCache_DnsServerId");

        builder.HasIndex(e => e.IsStale)
            .HasDatabaseName("IX_FilterListCache_IsStale");

        builder.HasIndex(e => e.LastSyncedAt)
            .HasDatabaseName("IX_FilterListCache_LastSyncedAt");
    }
}
