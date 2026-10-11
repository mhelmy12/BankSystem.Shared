using System;
using System.Reflection;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Shared.AspNetCore;
using Shared.Behaviors;
using Shared.Helpers;
using Shared.Services.CurrentUserService;
using Shared.Services.IdGeneratorService;

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

        services.AddHttpContextAccessor();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddCarter(new DependencyContextAssemblyCatalog(applicationAssemblies));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddKeyedScoped<ICurrentUserService, CurrentUserFromHeaders>("Headers");
        services.AddKeyedSingleton<IIdGeneratorService, SnowflakeIdGenerator>("Snowflake");
        return services;
    }

    public static WebApplication UseSharedInfrastructure(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.MapCarter();
        return app;
    }

}
