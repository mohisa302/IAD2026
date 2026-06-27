using IAD2026.Domain.Entities;

namespace IAD2026.Domain.ValueObjects;

public sealed class AccessType : ValueObject
{
    public static readonly AccessType None = new("None");
    public static readonly AccessType Read = new("Read");
    public static readonly AccessType Write = new("Write");
    public static readonly AccessType Modify = new("Modify");

    public string Value { get; }

    private AccessType(string value)
    {
        Value = value;
    }

    public static AccessType From(string? value)
    {
        return value?.Trim().ToLower() switch
        {
            "read" => Read,
            "write" => Write,
            "modify" => Modify,
            _ => None
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}