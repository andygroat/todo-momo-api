using Modules.Todos.Infrastructure.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using todo_momo_api.common.Domain;

namespace Modules.Todos.Domain;

/// <summary>
/// Represents a to-do item in the application.
/// </summary>
[ExcludeFromCodeCoverage]
[Table("TodoItems", Schema = Schemas.Default)]
public sealed class TodoItem : BusinessObject
{
    /// <summary>
    /// Gets or sets the description of the to-do item.
    /// </summary>
    [Required, MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the due date of the to-do item.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the to-do item is completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the date when the to-do item was completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }
}
