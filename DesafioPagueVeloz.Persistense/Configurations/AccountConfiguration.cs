using DesafioPagueVeloz.Domain.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioPagueVeloz.Persistense.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Operations)
            .WithOne()
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Currency)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
        builder.Navigation(x => x.Currency).AutoInclude();
        builder.Property(x => x.Balance).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CreditLimit).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ReservedAmount).HasColumnType("decimal(18,2)");
    }    
}