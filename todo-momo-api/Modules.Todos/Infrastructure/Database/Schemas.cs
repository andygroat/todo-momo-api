using System.Diagnostics.CodeAnalysis;

namespace Modules.Todos.Infrastructure.Database;

/// <summary>
/// The Schemas class contains constants for the database schema names used in the application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Schemas
{
    /// <summary>
    /// The schema name for the default related tables.
    /// </summary>
    public const string Default = "dbo";
}
