using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdGuard.DataAccess.Configurations;

/// <summary>
/// Entity configuration for <see cref="DeviceCacheEntity"/>.
/// </summary>
public class DeviceCacheConfiguration : IEntityTypeConfiguration<DeviceCacheEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DeviceCacheEntity> builder)
    {
        builder.ToTable("DeviceCache");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.DeviceId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.DeviceType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.DnsServerId)
            .HasMaxLength(100);

        builder.Property(e => e.DnsServerName)
            .HasMaxLength(200);

        builder.Property(e => e.ETag)
            .HasMaxLength(200);

        // Unique constraint on DeviceId
        builder.HasIndex(e => e.DeviceId)
            .IsUnique()
            .HasDatabaseName("UX_DeviceCache_DeviceId");

        // Indexes for common queries
        builder.HasIndex(e => e.DnsServerId)
            .HasDatabaseName("IX_DeviceCache_DnsServerId");

        builder.HasIndex(e => e.DeviceType)
            .HasDatabaseName("IX_DeviceCache_DeviceType");

        builder.HasIndex(e => e.IsStale)
            .HasDatabaseName("IX_DeviceCache_IsStale");

        builder.HasIndex(e => e.LastSyncedAt)
            .HasDatabaseName("IX_DeviceCache_LastSyncedAt");
    }
}
