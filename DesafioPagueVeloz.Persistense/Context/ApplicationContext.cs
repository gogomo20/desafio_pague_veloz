using System.Linq.Expressions;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Persistense.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DesafioPagueVeloz.Persistense.Context;

public class ApplicationContext : DbContext
{

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new OperationConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogsConfiguration());
        modelBuilder.ApplyConfiguration(new ErrorLogsConfiguration());
        modelBuilder.ApplyConfiguration(new CurrenciesConfiguration());

        #region DefaulFilterDeleteds

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "x");
                var property = Expression.Property(parameter, nameof(BaseEntity.Active));
                var constant = Expression.Constant(true);
                var body = Expression.Equal(property, constant);
                var lambda = Expression.Lambda(body, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                modelBuilder.Entity(entityType.ClrType).Property(nameof(BaseEntity.Active))
                    .HasDefaultValue(true)
                    .HasComment("true - Active, false - Deleted");
            }
        }

        #endregion
    }
}