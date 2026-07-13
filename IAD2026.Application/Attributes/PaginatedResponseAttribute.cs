namespace IAD2026.Application.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PaginatedResponseAttribute : Attribute
{
    /// <summary>
    /// The JSON property name that contains the list of items (e.g. "issues", "data", "results")
    /// </summary>
    public string ItemsProperty { get; set; } = "issues";

    /// <summary>
    /// The JSON property name that contains the total count (e.g. "total", "totalCount", "count")
    /// </summary>
    public string TotalProperty { get; set; } = "total";
}