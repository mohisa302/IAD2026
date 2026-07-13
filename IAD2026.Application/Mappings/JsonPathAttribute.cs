namespace IAD2026.Application.Mappings;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class JsonPathAttribute : Attribute
{
    public string Path { get; }

    public JsonPathAttribute(string path)
    {
        Path = path;
    }
}