namespace DesafioPagueVeloz.StartupInstaller;

public interface IInstaller
{
    /// <summary>
    /// Install services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    void InstallServices(IServiceCollection services, IConfiguration configuration);
}