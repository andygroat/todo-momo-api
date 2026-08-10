using Modules.Todos.Infrastructure.DependencyInjection;
using Serilog;

namespace todo_momo_api.Extensions;

internal static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Adds application building blocks to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add application building blocks to.</param>
    /// <returns>The WebApplicationBuilder with application building blocks added.</returns>
    public static WebApplicationBuilder AddApplicationBuilingBlocks(this WebApplicationBuilder builder)
    {
        // Add Serilog logging services
        builder.AddSerilogLogging();
        // Add the modules to the WebApplicationBuilder
        builder.AddModules();

        return builder;
    }

    /// <summary>
    /// Adds Serilog logging services to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add Serilog logging services to.</param>
    /// <returns>The WebApplicationBuilder with Serilog logging services added.</returns>
    private static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        // Configure Serilog as the logging provider for the application. Serilog is a structured logging library for .NET applications that allows for flexible and powerful logging capabilities, including support for various sinks (destinations) and structured log data.
        builder.Services.AddSerilog();

        return builder;
    }

    /// <summary>
    /// Adds the modules to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add modules to.</param>
    /// <returns>The WebApplicationBuilder with modules added.</returns>
    private static WebApplicationBuilder AddModules(this WebApplicationBuilder builder) 
    {
        // Add the Todo module services to the WebApplicationBuilder. This method registers the necessary services, configurations, and dependencies required for the Todo module to function properly within the application.
        builder.Services.AddTodoModule(builder.Configuration);

        return builder;
    }
}
