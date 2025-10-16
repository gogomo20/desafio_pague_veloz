using DesafioPagueVeloz.Persistense.Context;
using DesafioPagueVeloz.Persistense.Repositories;
using DesafioPagueVeloz.Persistense.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioPagueVeloz.Persistense.Extensions;

public static class PersistenseExtension
{
    public static void AddPersistense(this IServiceCollection builder, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection");
        builder.AddDbContext<ApplicationContext>(options => options.UseNpgsql(conn));
        builder.AddScoped<IUnitOfWork>(cfg =>
        {
            var dbContext = cfg.GetRequiredService<ApplicationContext>();
            return new UnitOfWork(dbContext);
        });

        builder.AddScoped(typeof(IReadableRepository<>), typeof(ReadableRepository<>));
        builder.AddScoped(typeof(IWriteableRepository<>), typeof(WriteableRepository<>));
        builder.AddTransient<ICurrencyRepository, CurrencyRepository>();

        builder.AddHostedService<OperationWorkerService>();
    }
}