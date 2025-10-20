using DesafioPagueVeloz.Application.Extensions;
using DesafioPagueVeloz.Persistense.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DesafilPagueVelos.Test.Helpers;

public class ApplicationHost
{
    public static IServiceProvider CreateServiceProvider(Action<ServiceCollection>? config = null)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        services.AddPersistense(configuration);
        services.AddApplication();
        services.AddLogging();
        return services.BuildServiceProvider();
    }
}