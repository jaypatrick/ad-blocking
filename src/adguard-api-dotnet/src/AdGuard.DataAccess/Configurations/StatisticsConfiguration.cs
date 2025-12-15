using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdGuard.DataAccess.Configurations;

/// <summary>
/// Entity configuration for <see cref="StatisticsEntity"/>.
/// </summary>
public class StatisticsConfiguration : IEntityTypeConfiguration<StatisticsEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<StatisticsEntity> builder)
    {
        builder.ToTable("Statistics");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.DnsServerId)
            .HasMaxLength(100);

        builder.Property(e => e.DeviceId)
            .HasMaxLength(100);

        builder.Property(e => e.Granularity)
            .HasConversion<int>();

        // Indexes for common queries
        builder.HasIndex(e => e.Date)
            .HasDatabaseName("IX_Statistics_Date");

        builder.HasIndex(e => e.DnsServerId)
            .HasDatabaseName("IX_Statistics_DnsServerId");

        builder.HasIndex(e => e.DeviceId)
            .HasDatabaseName("IX_Statistics_DeviceId");

        builder.HasIndex(e => e.Granularity)
            .HasDatabaseName("IX_Statistics_Granularity");

        // Composite index for common filter scenarios
        builder.HasIndex(e => new { e.Date, e.DnsServerId, e.Granularity })
            .HasDatabaseName("IX_Statistics_Date_DnsServerId_Granularity");

        // Unique constraint for statistics entries
        builder.HasIndex(e => new { e.Date, e.DnsServerId, e.DeviceId, e.Granularity })
            .IsUnique()
            .HasDatabaseName("UX_Statistics_DateServer");
    }
}
