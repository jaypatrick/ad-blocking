using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdGuard.DataAccess.Configurations;

/// <summary>
/// Entity configuration for <see cref="DnsServerCacheEntity"/>.
/// </summary>
public class DnsServerCacheConfiguration : IEntityTypeConfiguration<DnsServerCacheEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DnsServerCacheEntity> builder)
    {
        builder.ToTable("DnsServerCache");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.ServerId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ETag)
            .HasMaxLength(200);

        // Unique constraint on ServerId
        builder.HasIndex(e => e.ServerId)
            .IsUnique()
            .HasDatabaseName("UX_DnsServerCache_ServerId");

        // Indexes for common queries
        builder.HasIndex(e => e.IsDefault)
            .HasDatabaseName("IX_DnsServerCache_IsDefault");

        builder.HasIndex(e => e.IsStale)
            .HasDatabaseName("IX_DnsServerCache_IsStale");

        builder.HasIndex(e => e.LastSyncedAt)
            .HasDatabaseName("IX_DnsServerCache_LastSyncedAt");
    }
}
