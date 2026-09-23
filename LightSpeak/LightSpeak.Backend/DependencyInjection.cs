namespace LightSpeak.Backend;

using FluentValidation;
using LightSpeak.Backend.Common.Behaviors;
using LightSpeak.Backend.Infrastructure.Middleware;
using LightSpeak.Backend.Infrastructure.OpenApi;
using Microsoft.OpenApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "LightSpeak API",
                    Version = "v1",
                    Description =
                        "LightSpeak backend: auth, servers, channels, messages, " +
                        "plus SignalR ChatHub and VoiceHub (see hub tags).",
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token",
                };

                document.AddSignalRHubsDocumentation();

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }

    public static IServiceCollection AddGlobalExceptions(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
