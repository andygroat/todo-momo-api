using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Todos.Features;
using Modules.Todos.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Text;
using todo_momo_api.common.Behaviours;

namespace Modules.Todos.Infrastructure.DependencyInjection;

/// <summary>
/// Represents the Todo module and provides methods to register its services with the dependency injection container.
/// </summary>
public static class TodoModule
{
    /// <summary>
    /// Adds the Todo module services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The IConfiguration instance.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddTodoModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MediatR services from the current assembly
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(TodoModule).Assembly);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Register FluentValidation validators from the current assembly
        services.AddValidatorsFromAssembly(typeof(TodoModule).Assembly);

        // Configure the database context for the application

        // Configure to use an in-memory database for development and testing purposes. This is useful for scenarios where you want to quickly set up a database without the need for an actual database server.
        services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("TodoDb"));

        // In a production environment, you would typically configure the database context to use a real database provider (e.g., SQL Server, PostgreSQL, etc.) and provide the appropriate connection string from the configuration.
        // For example:
        // services.AddDbContext<TodoDbContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    /// <summary>
    /// Maps the endpoints related to to-do items in the WebApplication instance. This method can be used to configure the endpoints for creating, retrieving, updating, and deleting to-do items.
    /// </summary>
    /// <param name="app">The WebApplication instance used to map the endpoints.</param>
    /// <returns>The WebApplication instance with the mapped endpoints.</returns>
    public static WebApplication MapTodoEndpoints(this WebApplication app)
    {
        app.MapCreateTodoEndpoint();
        //app.MapGetTodosEndpoint();
        //app.MapGetTodoByIdEndpoint();
        //app.MapCompleteTodoEndpoint();

        return app;
    }
}
