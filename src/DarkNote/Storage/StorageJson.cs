using System.Text.Encodings.Web;
using System.Text.Json;
using DarkNote.Models;

namespace DarkNote.Storage;

/// <summary>The JSON format of every DarkNote data file (docs/data-design.md).</summary>
public static class StorageJson
{
    public static JsonSerializerOptions Options { get; } = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            // Keep files readable: don't escape '<' in FlowDocument XAML or non-ASCII note text.
            // Safe here because the files are never embedded in HTML.
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            // Hand-edited files: tolerate comments and trailing commas.
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            Converters =
            {
                new LenientEnumConverter<NoteColor>(NoteColor.Yellow),
                new LenientEnumConverter<NoteIcon>(NoteIcon.Notebook),
                new LenientEnumConverter<NoteFont>(NoteFont.SegoeUI),
                new LenientEnumConverter<FontSizePreset>(FontSizePreset.Medium),
                new LenientEnumConverter<SortOption>(SortOption.LastEdited),
                new NoteIconListConverter(),
            },
        };
        options.MakeReadOnly(populateMissingResolver: true);
        return options;
    }
}
