using DesafioPagueVeloz.Application.Extensions;
using DesafioPagueVeloz.Persistense.Extensions;
using DesafioPagueVeloz.Persistense.Workers;

namespace DesafioPagueVeloz.StartupInstaller;

public class APIInstallService : IInstaller
{
    public void InstallServices(IServiceCollection service, IConfiguration configuration)
    {
        service.AddPersistense(configuration);
        service.AddApplication();
        service.AddHostedService<OperationWorkerService>();
        service.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            
    }
}