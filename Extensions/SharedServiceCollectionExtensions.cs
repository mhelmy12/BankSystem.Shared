using System;
using System.Reflection;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Behaviors;
using Shared.Helpers;

namespace Shared.Extensions;

public static class SharedServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        Assembly[] applicationAssemblies,
        Action<MediatRServiceConfiguration> additionalMediatRConfig = null
        )
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(applicationAssemblies);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            if (additionalMediatRConfig != null)
            {
                additionalMediatRConfig(config);
            }
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddCarter();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static WebApplication UseSharedInfrastructure(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.MapCarter();
        return app;
    }

}
