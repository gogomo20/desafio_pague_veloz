using Microsoft.AspNetCore.Mvc;

namespace DesafioPagueVeloz.StartupInstaller;

public class MVCInstaller : IInstaller
{
    public void InstallServices(IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<ApiBehaviorOptions>(options => options.SuppressInferBindingSourcesForParameters = true);
    }
}