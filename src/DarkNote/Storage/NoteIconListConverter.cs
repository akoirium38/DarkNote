using System.Text.Json;
using System.Text.Json.Serialization;
using DarkNote.Models;

namespace DarkNote.Storage;

/// <summary>
/// Reads a list of icon names, dropping unknown entries (settings.json "recentIcons").
/// A missing list or a non-array becomes an empty list.
/// </summary>
public sealed class NoteIconListConverter : JsonConverter<List<NoteIcon>>
{
    public override bool HandleNull => true;

    public override List<NoteIcon> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var icons = new List<NoteIcon>();
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            reader.Skip();
            return icons;
        }

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType == JsonTokenType.String
                && LenientEnumConverter<NoteIcon>.TryParse(reader.GetString(), out var icon))
            {
                icons.Add(icon);
            }
            else
            {
                reader.Skip();
            }
        }

        return icons;
    }

    public override void Write(Utf8JsonWriter writer, List<NoteIcon> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var icon in value ?? [])
        {
            if (Enum.IsDefined(icon))
            {
                writer.WriteStringValue(LenientEnumConverter<NoteIcon>.GetName(icon));
            }
        }

        writer.WriteEndArray();
    }
}
