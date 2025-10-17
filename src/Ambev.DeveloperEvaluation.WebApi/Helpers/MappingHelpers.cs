using Ambev.DeveloperEvaluation.Domain.Shared;

namespace Ambev.DeveloperEvaluation.WebApi.Helpers;

public static class MappingHelpers
{
    /// <summary>
    /// Converts a validated sorting direction string (asc, desc, ascending, descending) 
    /// into the SortDirection enum (Asc, Desc).
    /// </summary>
    /// <param name="direction">The validated input string (e.g., "Descending").</param>
    /// <returns>The resulting SortDirection enum value, or null if the input is empty.</returns>
    public static SortDirection? ConvertToSortDirection(string? direction)
    {
        if (string.IsNullOrWhiteSpace(direction))
            return null;

        return direction.ToLower() switch
        {
            "asc" or "ascending" => SortDirection.Ascending,
            "desc" or "descending" => SortDirection.Descending,
            _ => null
        };
    }

    /// <summary>
    /// Converts a validated string field name into the SaleSortField enum.
    /// </summary>
    /// <param name="field">The validated input string (e.g., "CustomerName").</param>
    /// <returns>The resulting SaleSortField enum value, or null if the input is empty.</returns>
    public static SaleSortField? ConvertToSaleSortField(string? field)
    {
        if (string.IsNullOrWhiteSpace(field))
            return null;

        if (Enum.TryParse(field, true, out SaleSortField result))
            return result;

        return null;
    }
}