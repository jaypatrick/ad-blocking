using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdGuard.DataAccess.Configurations;

/// <summary>
/// Entity configuration for <see cref="AuditLogEntity"/>.
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuditLogEntity> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.OperationType)
            .HasConversion<int>();

        builder.Property(e => e.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.EntityId)
            .HasMaxLength(100);

        builder.Property(e => e.EntityName)
            .HasMaxLength(200);

        builder.Property(e => e.UserId)
            .HasMaxLength(100);

        builder.Property(e => e.Source)
            .HasMaxLength(100);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.ErrorCode)
            .HasMaxLength(50);

        builder.Property(e => e.ClientIpAddress)
            .HasMaxLength(50);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        // Indexes for common queries
        builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_AuditLogs_Timestamp");

        builder.HasIndex(e => e.OperationType)
            .HasDatabaseName("IX_AuditLogs_OperationType");

        builder.HasIndex(e => e.EntityType)
            .HasDatabaseName("IX_AuditLogs_EntityType");

        builder.HasIndex(e => e.EntityId)
            .HasDatabaseName("IX_AuditLogs_EntityId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId");

        builder.HasIndex(e => e.Success)
            .HasDatabaseName("IX_AuditLogs_Success");

        // Composite index for common filter scenarios
        builder.HasIndex(e => new { e.Timestamp, e.EntityType })
            .HasDatabaseName("IX_AuditLogs_Timestamp_EntityType");

        builder.HasIndex(e => new { e.EntityType, e.EntityId })
            .HasDatabaseName("IX_AuditLogs_EntityType_EntityId");
    }
}
