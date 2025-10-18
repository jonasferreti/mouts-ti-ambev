namespace Ambev.DeveloperEvaluation.Common.Cache;

/// <summary>
/// Centralized constants for cache configuration keys and prefixes.
/// </summary>
public static class CacheConstants
{
    /// <summary>
    /// The instance name/prefix used for all Sale-related keys in Redis.
    /// </summary>
    public const string SaleInstancePrefix = "Sales_";

    /// <summary>
    /// The fixed tag key used to group all Sale list query results for mass invalidation.
    /// </summary>
    public const string SalesListTag = "Sales_Lists";
}
