using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdGuard.DataAccess.Configurations;

/// <summary>
/// Entity configuration for <see cref="QueryLogEntity"/>.
/// </summary>
public class QueryLogConfiguration : IEntityTypeConfiguration<QueryLogEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<QueryLogEntity> builder)
    {
        builder.ToTable("QueryLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.ExternalId)
            .HasMaxLength(100);

        builder.Property(e => e.Domain)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.ResponseCode)
            .HasMaxLength(50);

        builder.Property(e => e.DeviceId)
            .HasMaxLength(100);

        builder.Property(e => e.DnsServerId)
            .HasMaxLength(100);

        builder.Property(e => e.BlockedReason)
            .HasMaxLength(500);

        builder.Property(e => e.QueryType)
            .HasMaxLength(20);

        builder.Property(e => e.ClientIp)
            .HasMaxLength(50);

        builder.Property(e => e.UpstreamServer)
            .HasMaxLength(200);

        builder.Property(e => e.FilterListName)
            .HasMaxLength(200);

        builder.Property(e => e.MatchedRule)
            .HasMaxLength(1000);

        // Indexes for common queries
        builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_QueryLogs_Timestamp");

        builder.HasIndex(e => e.Domain)
            .HasDatabaseName("IX_QueryLogs_Domain");

        builder.HasIndex(e => e.DeviceId)
            .HasDatabaseName("IX_QueryLogs_DeviceId");

        builder.HasIndex(e => e.DnsServerId)
            .HasDatabaseName("IX_QueryLogs_DnsServerId");

        builder.HasIndex(e => e.IsBlocked)
            .HasDatabaseName("IX_QueryLogs_IsBlocked");

        builder.HasIndex(e => e.ExternalId)
            .HasDatabaseName("IX_QueryLogs_ExternalId");

        // Composite index for common filter scenarios
        builder.HasIndex(e => new { e.Timestamp, e.DeviceId })
            .HasDatabaseName("IX_QueryLogs_Timestamp_DeviceId");

        builder.HasIndex(e => new { e.Timestamp, e.IsBlocked })
            .HasDatabaseName("IX_QueryLogs_Timestamp_IsBlocked");
    }
}
