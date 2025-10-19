using DesafioPagueVeloz.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DesafioPagueVeloz.StartupInstaller;

public class MVCInstaller : IInstaller
{
    public void InstallServices(IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<ApiBehaviorOptions>(options => options.SuppressInferBindingSourcesForParameters = true);
        service.AddControllers(options => options.Filters.Add<CustomExceptionFilter>());
    }
}