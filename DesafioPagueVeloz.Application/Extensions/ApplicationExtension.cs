using DesafioPagueVeloz.Application.Behaviors;
using DesafioPagueVeloz.Application.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioPagueVeloz.Application.Extensions;

public static class ApplicationExtension
{
        public static void AddApplication(this IServiceCollection service)
        {
                service.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationExtension).Assembly));
                service.AddValidatorsFromAssembly(typeof(ApplicationExtension).Assembly);
                service.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                service.AddTransient(typeof(IPipelineBehavior<,>), typeof(RasterErrorsBehavior<,>));

        }
}