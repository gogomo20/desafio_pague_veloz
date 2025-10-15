using DesafioPagueVeloz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioPagueVeloz.Persistense.Configurations;

public class ErrorLogsConfiguration : IEntityTypeConfiguration<ErrorLogs>
{
    public void Configure(EntityTypeBuilder<ErrorLogs> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Message).IsRequired();
        builder.Property(x => x.Data).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}