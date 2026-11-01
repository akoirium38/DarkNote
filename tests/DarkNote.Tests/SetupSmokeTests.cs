using System.Windows.Documents;

namespace DarkNote.Tests;

/// <summary>
/// Checks that the test project is wired up correctly (xUnit v3 + WPF on an STA thread).
/// </summary>
public class SetupSmokeTests
{
    [Fact]
    public void Xunit_Runs()
    {
        Assert.Equal(2, 1 + 1);
    }

    [WpfFact]
    public void WpfFact_RunsOnStaThread_AndCanCreateFlowDocument()
    {
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());

        var document = new FlowDocument(new Paragraph(new Run("hello")));

        Assert.Equal("hello", new TextRange(document.ContentStart, document.ContentEnd).Text.TrimEnd());
    }
}
