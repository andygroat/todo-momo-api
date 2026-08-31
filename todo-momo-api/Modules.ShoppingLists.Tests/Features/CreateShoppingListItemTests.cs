using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Features;
using todo_momo_api.common.Constants;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="CreateShoppingListItem"/> vertical slice.
/// </summary>
public class CreateShoppingListItemTests
{
    [Test]
    public async Task Handle_WhenListExists_PersistsItemAndReturnsSuccessWithId()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        context.ShoppingLists.Add(list);
        await context.SaveChangesAsync();

        var handler = new CreateShoppingListItem.Handler(context, NullLogger<CreateShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new CreateShoppingListItem.Command(list.Id, "Milk"), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsNotEqualTo(Guid.Empty);

        var stored = await context.ShoppingListItems.SingleAsync();
        await Assert.That(stored.Id).IsEqualTo(result.Value);
        await Assert.That(stored.ShoppingListId).IsEqualTo(list.Id);
        await Assert.That(stored.Title).IsEqualTo("Milk");
        await Assert.That(stored.IsComplete).IsFalse();
    }

    [Test]
    public async Task Handle_WhenListDoesNotExist_ReturnsNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var handler = new CreateShoppingListItem.Handler(context, NullLogger<CreateShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new CreateShoppingListItem.Command(Guid.NewGuid(), "Milk"), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingList.NotFound");
    }

    [Test]
    public async Task Handle_WhenListIsDeleted_ReturnsNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Deleted };
        context.ShoppingLists.Add(list);
        await context.SaveChangesAsync();

        var handler = new CreateShoppingListItem.Handler(context, NullLogger<CreateShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new CreateShoppingListItem.Command(list.Id, "Milk"), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingList.NotFound");
    }
}

/// <summary>
/// Tests for the <see cref="CreateShoppingListItem.Validator"/> FluentValidation rules.
/// </summary>
public class CreateShoppingListItemValidatorTests
{
    [Test]
    public async Task Validator_WhenTitleIsEmpty_FailsValidation()
    {
        var validator = new CreateShoppingListItem.Validator();

        var result = await validator.ValidateAsync(new CreateShoppingListItem.CreateShoppingListItemCommand(string.Empty));

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    public async Task Validator_WhenTitleExceedsMaxLength_FailsValidation()
    {
        var validator = new CreateShoppingListItem.Validator();

        var result = await validator.ValidateAsync(new CreateShoppingListItem.CreateShoppingListItemCommand(new string('x', 201)));

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    public async Task Validator_WhenCommandIsValid_PassesValidation()
    {
        var validator = new CreateShoppingListItem.Validator();

        var result = await validator.ValidateAsync(new CreateShoppingListItem.CreateShoppingListItemCommand("Milk"));

        await Assert.That(result.IsValid).IsTrue();
    }
}
