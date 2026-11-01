using System.Windows;
using System.Windows.Media;

namespace DarkNote.Tests;

/// <summary>
/// Checks that the bundled UI assets (Lucide icons, fonts) are packed into the app and can be loaded.
/// </summary>
public class ResourceSmokeTests
{
    private const string Component = "pack://application:,,,/DarkNote;component/";

    static ResourceSmokeTests()
    {
        // Application's static constructor registers the pack:// scheme and the "application" package,
        // which pack://application URIs need when no WPF Application is running (as in tests).
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public static TheoryData<string> PresetIcons =>
    [
        "Info", "Calendar", "Lightbulb", "Lock", "Database", "Folder", "Gift", "BotMessageSquare",
        "CodeXml", "Notebook", "Cat", "MailOpen", "Fish", "Book", "Bookmark", "BookText",
        "List", "Link", "Play", "Store", "Languages", "Copy",
    ];

    [WpfTheory]
    [MemberData(nameof(PresetIcons))]
    public void LucideIcon_IsGeometry_InsideTheIconCanvas(string name)
    {
        var icons = new ResourceDictionary { Source = new Uri(Component + "Resources/Icons/LucideIcons.xaml") };

        var geometry = Assert.IsAssignableFrom<Geometry>(icons["Icon." + name]);

        var bounds = geometry.Bounds;
        Assert.False(bounds.IsEmpty);
        Assert.InRange(bounds.Left, 0, 24);
        Assert.InRange(bounds.Top, 0, 24);
        Assert.InRange(bounds.Right, 0, 24);
        Assert.InRange(bounds.Bottom, 0, 24);
    }

    [WpfTheory]
    [InlineData("JetBrains Mono")]
    [InlineData("Caveat")]
    public void BundledFont_IsFoundInAppResources(string familyName)
    {
        var families = Fonts.GetFontFamilies(new Uri(Component + "Resources/Fonts/"));

        Assert.Contains(families, f => f.Source.EndsWith("#" + familyName, StringComparison.Ordinal));
    }
}
