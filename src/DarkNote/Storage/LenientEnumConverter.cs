using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DarkNote.Storage;

/// <summary>
/// Reads and writes an enum as its name (or its [JsonStringEnumMemberName]). On read, names match
/// case-insensitively, and anything unknown (other names, numbers, null, objects) becomes
/// <paramref name="fallback"/> instead of failing the whole file (workflow 2.4.3, edge case 7).
/// </summary>
public sealed class LenientEnumConverter<T>(T fallback) : JsonConverter<T>
    where T : struct, Enum
{
    private static readonly Dictionary<string, T> ByName = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<T, string> NameOf = [];

    static LenientEnumConverter()
    {
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var value = (T)field.GetValue(null)!;
            var name = field.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name ?? field.Name;
            ByName[name] = value;
            NameOf[value] = name;
        }
    }

    public T Fallback { get; } = fallback;

    // Null tokens must reach Read, so null becomes the fallback.
    public override bool HandleNull => true;

    /// <summary>Parses a saved name; false for unknown names.</summary>
    public static bool TryParse(string? name, out T value)
    {
        value = default;
        return name is not null && ByName.TryGetValue(name, out value);
    }

    public static string GetName(T value) => NameOf[value];

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && TryParse(reader.GetString(), out var value))
        {
            return value;
        }

        reader.Skip(); // no-op for single tokens; skips whole objects/arrays
        return Fallback;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(NameOf.TryGetValue(value, out var name) ? name : NameOf[Fallback]);
    }
}
