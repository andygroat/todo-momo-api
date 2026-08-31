using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Infrastructure.Database;
using todo_momo_api.common.ResultHelper;

namespace Modules.ShoppingLists.Features;

/// <summary>
/// CreateShoppingList feature slice for creating a new shopping list.
/// </summary>
public static class CreateShoppingList
{
    /// <summary>
    /// Command record representing the data required to create a new shopping list.
    /// </summary>
    /// <param name="Title">The title of the shopping list.</param>
    public record CreateShoppingListCommand(string Title) : IRequest<Result<Guid>>;

    /// <summary>
    /// Validator class for validating the CreateShoppingList command.
    /// </summary>
    public sealed class Validator : AbstractValidator<CreateShoppingListCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        }
    }

    /// <summary>
    /// Handler class for processing the CreateShoppingList command.
    /// </summary>
    /// <param name="context">The database context used to interact with shopping lists in the database.</param>
    /// <param name="logger">The logger used to log information about the creation of the shopping list.</param>
    internal sealed class Handler(ShoppingListDbContext context, ILogger<Handler> logger) : IRequestHandler<CreateShoppingListCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateShoppingListCommand request, CancellationToken cancellationToken)
        {
            var shoppingList = new ShoppingList
            {
                Id = Guid.NewGuid(),
                Title = request.Title
            };

            context.ShoppingLists.Add(shoppingList);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Created ShoppingList {ShoppingListId}: {ShoppingListTitle}",
                shoppingList.Id, shoppingList.Title);

            return Result.Success(shoppingList.Id);
        }
    }

    /// <summary>
    /// Maps the CreateShoppingList command to an HTTP POST endpoint at "/api/shoppinglists".
    /// </summary>
    /// <param name="app">The WebApplication instance used to map the endpoint.</param>
    public static WebApplication MapCreateShoppingListEndpoint(this WebApplication app)
    {
        app.MapPost("/api/shoppinglists", async (CreateShoppingListCommand command, IMediator mediator, CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? Results.Created($"/api/shoppinglists/{result.Value}", new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("CreateShoppingList")
        .WithTags("shoppinglists");

        return app;
    }
}
