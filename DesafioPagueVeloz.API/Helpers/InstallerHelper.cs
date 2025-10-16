using System.Reflection;
using DesafioPagueVeloz.StartupInstaller;

namespace DesafioPagueVeloz.Helpers;

public static class InstallerHelper
{
    public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
    {
        var installers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IInstaller>().ToList();

        installers.ForEach(installer => installer.InstallServices(services, configuration));
    }
}