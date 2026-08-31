using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.ShoppingLists.Features;
using Modules.ShoppingLists.Infrastructure.Database;
using System.Diagnostics.CodeAnalysis;
using todo_momo_api.common.Behaviours;

namespace Modules.ShoppingLists.Infrastructure.DependencyInjection;

/// <summary>
/// Represents the ShoppingLists module and provides methods to register its services with the dependency injection container.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ShoppingListsModule
{
    /// <summary>
    /// Adds the ShoppingLists module services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddShoppingListsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(ShoppingListsModule).Assembly);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(ShoppingListsModule).Assembly);

        services.AddDbContext<ShoppingListDbContext>(options => options.UseInMemoryDatabase("ShoppingListsDb"));

        return services;
    }

    /// <summary>
    /// Maps the endpoints related to shopping lists and their items in the <see cref="WebApplication"/> instance.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance used to map the endpoints.</param>
    /// <returns>The <see cref="WebApplication"/> instance with the mapped endpoints.</returns>
    public static WebApplication MapShoppingListsEndpoints(this WebApplication app)
    {
        app.MapCreateShoppingListEndpoint();
        app.MapGetShoppingListsEndpoint();
        app.MapGetShoppingListByIdEndpoint();
        app.MapUpdateShoppingListEndpoint();
        app.MapDeleteShoppingListEndpoint();

        app.MapCreateShoppingListItemEndpoint();
        app.MapGetShoppingListItemByIdEndpoint();
        app.MapGetShoppingListItemsEndpoint();
        app.MapUpdateShoppingListItemEndpoint();
        app.MapDeleteShoppingListItemEndpoint();

        return app;
    }
}
