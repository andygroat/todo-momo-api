using Modules.Todos.Infrastructure.DependencyInjection;
using Scalar.AspNetCore;
using System.Diagnostics.CodeAnalysis;

namespace todo_momo_api.Extensions;

[ExcludeFromCodeCoverage]
internal static class WebApplicationExtensions
{
    /// <summary>
    /// Maps the WebApplication instance and returns it. This method can be used to configure additional middleware, endpoints, or other application-specific settings.
    /// </summary>
    /// <param name="app">The WebApplication instance to map.</param>
    /// <returns>The mapped WebApplication instance.</returns>
    public static WebApplication MapWebApplication(this WebApplication app)
    {
        // If the application is in development environment, map the OpenAPI endpoints and the Scalar API reference for the WebApplication instance.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApiWithScalarPage();
        }

        // Map the endpoints related to the modules in the WebApplication instance.
        app.MapModuleEndpoints();

        return app;
    }

    /// <summary>
    /// Maps the OpenAPI endpoints and the Scalar API reference for the WebApplication instance. This method can be used to configure OpenAPI documentation and Scalar API reference for the application.
    /// </summary>
    /// <param name="app">The WebApplication instance to map.</param>
    /// <returns>The mapped WebApplication instance.</returns>
    private static WebApplication MapOpenApiWithScalarPage(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        return app;
    }

    /// <summary>
    /// Maps the endpoints related to the modules in the WebApplication instance. This method can be used to configure the endpoints for the modules in the application.
    /// </summary>
    /// <param name="app">The WebApplication instance to map.</param>
    /// <returns>The mapped WebApplication instance.</returns>
    private static WebApplication MapModuleEndpoints(this WebApplication app) 
    {
        app.MapTodoEndpoints();

        return app;
    }
}
