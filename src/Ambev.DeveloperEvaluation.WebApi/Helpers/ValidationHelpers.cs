namespace Ambev.DeveloperEvaluation.Application.Helpers;

public static class ValidationHelpers
{
    /// <summary>
    /// Checks if the provided string is a valid sort direction alias, 
    /// accepting both short (asc/desc) and long (ascending/descending) forms.
    /// </summary>
    /// <param name="direction">The input string from the query parameter.</param>
    /// <returns>True if the string matches an allowed alias, ignoring case; otherwise, false.</returns>
    public static bool IsValidSortDirection(string? direction)
    {
        if (string.IsNullOrWhiteSpace(direction))
            return false;

        return direction.ToLower() switch
        {
            "asc" or "ascending" or "desc" or "descending" => true,
            _ => false
        };
    }

    /// <summary>
    /// Checks if the provided string matches any member name of the specified enum type (TEnum).
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum to check against.</typeparam>
    /// <param name="value">The input string</param>
    /// <returns>True if the string matches an enum name (case-insensitive); otherwise, false.</returns>
    public static bool IsValidEnumName<TEnum>(string? value) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        return Enum.TryParse<TEnum>(value, true, out _);
    }

    /// <summary>
    /// Generates a dynamic error message listing all accepted members of an Enum.
    /// </summary>
    /// <typeparam name="TEnum">The enum type (e.g., SaleSortField).</typeparam>
    /// <param name="fieldName">The input field name</param>
    /// <returns>A formatted error message listing all valid enum members.</returns>
    public static string GetInvalidEnumMessage<TEnum>(string fieldName) where TEnum : Enum
    {
        var validFields = Enum.GetNames(typeof(TEnum));
        var validFieldList = string.Join(", ", validFields);

        // 3. Retorna a mensagem completa.
        return $"The allowed values for {fieldName} are : {validFieldList}";
    }

    /// <summary>
    /// Generates the standard error message for an invalid sort direction value.
    /// </summary>
    /// <param name="fieldName">The input field name</param>
    /// <returns>A formatted error message listing accepted values.</returns>
    public static string GetInvalidSortDirectionMessage(string fieldName)
    {
        // As opções válidas são fixas e não dependem de um Enum.GetNames
        string validFieldList = "asc, ascending, desc, descending";

        return $"The allowed values for {fieldName} are : {validFieldList}";
    }
}
