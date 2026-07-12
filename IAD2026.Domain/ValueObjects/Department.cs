namespace IAD2026.Domain.ValueObjects;

public sealed class Department : ValueObject
{
    public string Value { get; }

    private Department(string value)
    {
        Value = value;
    }

    public static Department From(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new Department("None");

        return new Department(value.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}