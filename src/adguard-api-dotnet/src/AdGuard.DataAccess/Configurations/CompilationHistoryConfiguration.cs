using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdGuard.DataAccess.Configurations;

/// <summary>
/// Entity configuration for <see cref="CompilationHistoryEntity"/>.
/// </summary>
public class CompilationHistoryConfiguration : IEntityTypeConfiguration<CompilationHistoryEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CompilationHistoryEntity> builder)
    {
        builder.ToTable("CompilationHistory");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.ConfigurationPath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.ConfigurationFormat)
            .HasMaxLength(20);

        builder.Property(e => e.FilterListName)
            .HasMaxLength(200);

        builder.Property(e => e.OutputPath)
            .HasMaxLength(1000);

        builder.Property(e => e.OutputHash)
            .HasMaxLength(100);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(4000);

        builder.Property(e => e.CompilerVersion)
            .HasMaxLength(50);

        builder.Property(e => e.HostlistCompilerVersion)
            .HasMaxLength(50);

        builder.Property(e => e.MachineName)
            .HasMaxLength(100);

        // Ignore computed property
        builder.Ignore(e => e.Duration);

        // Indexes for common queries
        builder.HasIndex(e => e.StartedAt)
            .HasDatabaseName("IX_CompilationHistory_StartedAt");

        builder.HasIndex(e => e.Success)
            .HasDatabaseName("IX_CompilationHistory_Success");

        builder.HasIndex(e => e.FilterListName)
            .HasDatabaseName("IX_CompilationHistory_FilterListName");

        builder.HasIndex(e => e.OutputHash)
            .HasDatabaseName("IX_CompilationHistory_OutputHash");

        // Composite index for common filter scenarios
        builder.HasIndex(e => new { e.StartedAt, e.Success })
            .HasDatabaseName("IX_CompilationHistory_StartedAt_Success");
    }
}
