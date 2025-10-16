using DesafioPagueVeloz.Persistense.Extensions;

namespace DesafioPagueVeloz.StartupInstaller;

public class APIInstallService
{
    public void InstallServices(IServiceCollection service, IConfiguration configuration)
    {
        service.AddPersistense(configuration);
        service.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            
    }
}