using System.Text.Json.Serialization;

namespace DarkNote.Models;

// Enums from docs/data-design.md. Saved as strings; the JSON name is the member name
// unless [JsonStringEnumMemberName] says otherwise. Defaults are applied by StorageJson.

/// <summary>Banner color of a regular note (PRD 2.2.5). Dark gray is Clipboard-only and not listed.</summary>
public enum NoteColor
{
    Yellow,
    Red,
    Blue,
    White,
    Green,
    Purple,
    Pink,
    Orange,
    Gray,
}

/// <summary>Note font (PRD 2.2.4).</summary>
public enum NoteFont
{
    SegoeUI,
    JetBrainsMono,
    Caveat,
}

/// <summary>Font size preset (PRD 2.2.4).</summary>
public enum FontSizePreset
{
    Small,
    Medium,
    Large,
}

/// <summary>Main page sort choice (PRD 2.1.3).</summary>
public enum SortOption
{
    LastEdited,
    Color,
    Name,
    Icon,
}

/// <summary>
/// Note icon presets (PRD 2.2.5), in preset order. JSON names are the Lucide icon names.
/// The Clipboard Note's "copy" icon is fixed in code and not listed.
/// </summary>
public enum NoteIcon
{
    [JsonStringEnumMemberName("info")] Info,
    [JsonStringEnumMemberName("calendar")] Calendar,
    [JsonStringEnumMemberName("lightbulb")] Lightbulb,
    [JsonStringEnumMemberName("lock")] Lock,
    [JsonStringEnumMemberName("database")] Database,
    [JsonStringEnumMemberName("folder")] Folder,
    [JsonStringEnumMemberName("gift")] Gift,
    [JsonStringEnumMemberName("bot-message-square")] BotMessageSquare,
    [JsonStringEnumMemberName("code-xml")] CodeXml,
    [JsonStringEnumMemberName("notebook")] Notebook,
    [JsonStringEnumMemberName("cat")] Cat,
    [JsonStringEnumMemberName("mail-open")] MailOpen,
    [JsonStringEnumMemberName("fish")] Fish,
    [JsonStringEnumMemberName("book")] Book,
    [JsonStringEnumMemberName("bookmark")] Bookmark,
    [JsonStringEnumMemberName("book-text")] BookText,
    [JsonStringEnumMemberName("list")] List,
    [JsonStringEnumMemberName("link")] Link,
    [JsonStringEnumMemberName("play")] Play,
    [JsonStringEnumMemberName("store")] Store,
    [JsonStringEnumMemberName("languages")] Languages,
}
