using System.Reflection;
using System.Text.Json;

namespace IAD2026.Application.Mappings;

public static class JsonElementMapper
{
    public static T Map<T>(JsonElement element) where T : new()
    {
        var instance = new T();
        MapProperties(element, instance);
        return instance;
    }

    public static void MapProperties(JsonElement element, object instance)
    {
        var type = instance.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (!prop.CanWrite)
                continue;

            try
            {
                var jsonPathAttr = prop.GetCustomAttribute<JsonPathAttribute>();
                string path = jsonPathAttr?.Path ?? prop.Name;

                if (!TryGetValueByPath(element, path, out var jsonValue))
                {
                    SetDefaultValue(prop, instance);
                    continue;
                }

                if (jsonValue.ValueKind == JsonValueKind.Null)
                {
                    SetDefaultValue(prop, instance);
                    continue;
                }

                object? value = ConvertValue(jsonValue, prop.PropertyType);
                prop.SetValue(instance, value);
            }
            catch
            {
                // Never let one bad property break the whole object
                SetDefaultValue(prop, instance);
            }
        }
    }

    private static bool TryGetValueByPath(JsonElement root, string path, out JsonElement value)
    {
        value = default;
        if (string.IsNullOrWhiteSpace(path)) return false;

        JsonElement current = root;
        foreach (var segment in path.Split('.'))
        {
            if (current.ValueKind != JsonValueKind.Object) return false;
            if (!current.TryGetProperty(segment, out current)) return false;
            if (current.ValueKind == JsonValueKind.Null) break;
        }

        value = current;
        return true;
    }

    /// <summary>
    /// Sets a safe default. For string → "", for nullable/reference types → null
    /// </summary>
    private static void SetDefaultValue(PropertyInfo prop, object instance)
    {
        var targetType = prop.PropertyType;

        if (targetType == typeof(string))
        {
            prop.SetValue(instance, string.Empty);
            return;
        }

        if (Nullable.GetUnderlyingType(targetType) != null || !targetType.IsValueType)
        {
            prop.SetValue(instance, null);
        }
    }

    private static object? ConvertValue(JsonElement jsonValue, Type targetType)
    {
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // ==================== STRING ====================
        if (jsonValue.ValueKind == JsonValueKind.String)
        {
            string? str = jsonValue.GetString();

            if (underlyingType == typeof(string))
                return str ?? string.Empty;

            if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTime?))
            {
                if (string.IsNullOrWhiteSpace(str)) return null;

                // Robust parsing for format (supports +0330 and +03:30)
                if (DateTime.TryParse(str, out var dt))
                    return dt;

                // Extra formats 
                string[] formats = {
                    "yyyy-MM-ddTHH:mm:ss.fffzzz",
                    "yyyy-MM-ddTHH:mm:ss.fffZ",
                    "yyyy-MM-ddTHH:mm:ss.fffK"
                };

                if (DateTime.TryParseExact(str, formats,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                {
                    return dt;
                }

                return null;
            }

            if (underlyingType == typeof(DateTimeOffset) || underlyingType == typeof(DateTimeOffset?))
            {
                if (DateTimeOffset.TryParse(str, out var dto)) return dto;
                return null;
            }

            if (underlyingType == typeof(Guid) || underlyingType == typeof(Guid?))
            {
                if (Guid.TryParse(str, out var guid)) return guid;
                return null;
            }
        }

        // ==================== NUMBERS ====================
        if (jsonValue.ValueKind == JsonValueKind.Number)
        {
            if (underlyingType == typeof(int) || underlyingType == typeof(int?))
                return jsonValue.GetInt32();
            if (underlyingType == typeof(long) || underlyingType == typeof(long?))
                return jsonValue.GetInt64();
            if (underlyingType == typeof(double) || underlyingType == typeof(double?))
                return jsonValue.GetDouble();
            if (underlyingType == typeof(decimal) || underlyingType == typeof(decimal?))
                return jsonValue.GetDecimal();
        }

        // ==================== BOOLEAN ====================
        if (jsonValue.ValueKind == JsonValueKind.True || jsonValue.ValueKind == JsonValueKind.False)
        {
            if (underlyingType == typeof(bool) || underlyingType == typeof(bool?))
                return jsonValue.GetBoolean();
        }

        // ==================== OBJECT / ARRAY ====================
        if (jsonValue.ValueKind == JsonValueKind.Object)
            return MapNestedObject(jsonValue, targetType);

        if (jsonValue.ValueKind == JsonValueKind.Array)
            return MapArray(jsonValue, targetType);

        // Fallback
        return JsonSerializer.Deserialize(jsonValue.GetRawText(), targetType);
    }

    private static object? MapNestedObject(JsonElement jsonValue, Type targetType)
    {
        if (targetType == typeof(string))
            return jsonValue.GetRawText();

        if (targetType.IsClass && targetType != typeof(string))
        {
            var nested = Activator.CreateInstance(targetType);
            if (nested != null)
            {
                MapProperties(jsonValue, nested);
                return nested;
            }
        }

        return JsonSerializer.Deserialize(jsonValue.GetRawText(), targetType);
    }

    private static object? MapArray(JsonElement jsonValue, Type targetType)
    {
        if (!targetType.IsGenericType) return null;

        var elementType = targetType.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = (System.Collections.IList)Activator.CreateInstance(listType)!;

        foreach (var item in jsonValue.EnumerateArray())
        {
            list.Add(item.ValueKind == JsonValueKind.Null ? null : ConvertValue(item, elementType));
        }

        return list;
    }
}