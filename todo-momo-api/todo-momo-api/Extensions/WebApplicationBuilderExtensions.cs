using Modules.Todos.Infrastructure.DependencyInjection;
using Serilog;
using todo_momo_api.Exceptions;

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
        // Add exception handling middleware to the application
        builder.AddExceptionHandling();
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

    /// <summary>
    /// Adds exception handling middleware to the application. This middleware is responsible for catching unhandled exceptions that occur during the processing of HTTP requests and generating appropriate error responses. It can be used to provide consistent error handling and logging throughout the application.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to add the exception handling middleware to.</param>
    /// <returns>The WebApplicationBuilder with the exception handling middleware added.</returns>
    private static WebApplicationBuilder AddExceptionHandling(this WebApplicationBuilder builder)
    {
        // Configure the validation exception handler to handle FluentValidation exceptions and generate appropriate error responses. This is useful for scenarios where you want to provide detailed information about validation errors to clients in a standardized format.
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();

        // Configure the global exception handler to handle unhandled exceptions and generate appropriate error responses. This is a global exception handler that will catch any unhandled exceptions that occur during the processing of HTTP requests and provide a standardized error response to clients. Any specific exception handlers (like ValidationExceptionHandler) should be registered before the global handler.
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // Add ProblemDetails middleware to the application. ProblemDetails is a standardized format for representing error responses in HTTP APIs, as defined by RFC 9457. It provides a consistent way to convey error information to clients, including details about the error type, status code, and additional context.
        // If this line is not included, the dependency injection container will not be able to resolve the IProblemDetailsService, which is required by the exception handlers to generate ProblemDetails responses.
        builder.Services.AddProblemDetails();

        return builder;
    }
}
