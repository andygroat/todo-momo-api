using Modules.ShoppingLists.Infrastructure.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using todo_momo_api.common.Domain;

namespace Modules.ShoppingLists.Domain;

/// <summary>
/// Represents a single item within a <see cref="ShoppingList"/>.
/// </summary>
[ExcludeFromCodeCoverage]
[Table("ShoppingListItems", Schema = Schemas.Default)]
public sealed class ShoppingListItem : BusinessObject
{
    /// <summary>
    /// Gets or sets the identifier of the owning <see cref="ShoppingList"/>.
    /// </summary>
    [Required]
    public Guid ShoppingListId { get; set; }

    /// <summary>
    /// Gets or sets the title of the shopping list item.
    /// </summary>
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the item has been completed.
    /// </summary>
    public bool IsComplete { get; set; }
}
