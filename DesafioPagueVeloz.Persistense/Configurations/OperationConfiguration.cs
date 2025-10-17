using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioPagueVeloz.Persistense.Configurations;

public class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Account)
            .WithMany(x => x.Operations)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        builder.HasOne(x => x.TransferAccount)
            .WithMany()
            .HasForeignKey(x => x.TransferId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);
        builder.HasOne(x => x.Currency)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
        builder.Navigation(x => x.Currency).AutoInclude();
        builder.Navigation(x => x.Account).AutoInclude();
    }
}