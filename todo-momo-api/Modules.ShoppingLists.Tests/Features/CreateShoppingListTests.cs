using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Features;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="CreateShoppingList"/> vertical slice.
/// </summary>
public class CreateShoppingListTests
{
    [Test]
    public async Task Handle_WithValidCommand_PersistsListAndReturnsSuccessWithId()
    {
        // Arrange
        await using var context = TestContextFactory.Create();
        var handler = new CreateShoppingList.Handler(context, NullLogger<CreateShoppingList.Handler>.Instance);
        var command = new CreateShoppingList.CreateShoppingListCommand("Groceries");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsNotEqualTo(Guid.Empty);

        var stored = await context.ShoppingLists.SingleAsync();
        await Assert.That(stored.Id).IsEqualTo(result.Value);
        await Assert.That(stored.Title).IsEqualTo("Groceries");
    }
}

/// <summary>
/// Tests for the <see cref="CreateShoppingList.Validator"/> FluentValidation rules.
/// </summary>
public class CreateShoppingListValidatorTests
{
    [Test]
    public async Task Validator_WhenTitleIsEmpty_FailsValidation()
    {
        var validator = new CreateShoppingList.Validator();

        var result = await validator.ValidateAsync(new CreateShoppingList.CreateShoppingListCommand(string.Empty));

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    public async Task Validator_WhenTitleExceedsMaxLength_FailsValidation()
    {
        var validator = new CreateShoppingList.Validator();
        var tooLong = new string('x', 101);

        var result = await validator.ValidateAsync(new CreateShoppingList.CreateShoppingListCommand(tooLong));

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    public async Task Validator_WhenCommandIsValid_PassesValidation()
    {
        var validator = new CreateShoppingList.Validator();

        var result = await validator.ValidateAsync(new CreateShoppingList.CreateShoppingListCommand("Groceries"));

        await Assert.That(result.IsValid).IsTrue();
    }
}
