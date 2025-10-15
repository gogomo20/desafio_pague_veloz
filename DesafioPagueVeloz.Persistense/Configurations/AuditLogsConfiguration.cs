using DesafioPagueVeloz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioPagueVeloz.Persistense.Configurations;

public class AuditLogsConfiguration : IEntityTypeConfiguration<AuditLogs>
{
    public void Configure(EntityTypeBuilder<AuditLogs> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Entity).IsRequired();
        builder.Property(x => x.OldValue).IsRequired();
        builder.Property(x => x.NewValue).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}