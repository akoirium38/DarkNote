#:property TargetFramework=net10.0-windows
#:property UseWPF=true
#:property PublishAot=false
#:property PublishTrimmed=false
// Renders assets/branding/DarkNote.svg (simple 24x24 path-only SVG) into a multi-size PNG-compressed .ico.
// Usage: dotnet run make-ico.cs -- <in.svg> <out.ico> <preview-dir>
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

var svgPath = args[0];
var icoPath = args[1];
var previewDir = args[2];
int[] sizes = [256, 64, 48, 40, 32, 24, 20, 16];

var thread = new Thread(() =>
{
    var svg = XDocument.Load(svgPath).Root!;
    XNamespace ns = "http://www.w3.org/2000/svg";
    var rootStroke = (string?)svg.Attribute("stroke") ?? "none";
    var rootFill = (string?)svg.Attribute("fill") ?? "black";
    var strokeWidth = double.Parse((string?)svg.Attribute("stroke-width") ?? "1");

    Brush? ToBrush(string value) =>
        value == "none" ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));

    var pngs = new List<byte[]>();
    foreach (var size in sizes)
    {
        var canvas = new System.Windows.Controls.Canvas { Width = 24, Height = 24 };
        foreach (var p in svg.Elements(ns + "path"))
        {
            canvas.Children.Add(new System.Windows.Shapes.Path
            {
                Data = Geometry.Parse((string)p.Attribute("d")!),
                Fill = ToBrush((string?)p.Attribute("fill") ?? rootFill),
                Stroke = ToBrush((string?)p.Attribute("stroke") ?? rootStroke),
                StrokeThickness = strokeWidth,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
            });
        }
        var box = new System.Windows.Controls.Viewbox { Child = canvas, Width = size, Height = size };
        box.Measure(new Size(size, size));
        box.Arrange(new Rect(0, 0, size, size));
        box.UpdateLayout();

        var bmp = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
        bmp.Render(box);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bmp));
        using var ms = new MemoryStream();
        encoder.Save(ms);
        pngs.Add(ms.ToArray());
        File.WriteAllBytes(System.IO.Path.Combine(previewDir, $"DarkNote-{size}.png"), ms.ToArray());
    }

    // ICO container: ICONDIR + ICONDIRENTRY[] + PNG payloads.
    using var fs = File.Create(icoPath);
    using var w = new BinaryWriter(fs);
    w.Write((ushort)0); w.Write((ushort)1); w.Write((ushort)sizes.Length);
    var offset = 6 + 16 * sizes.Length;
    for (var i = 0; i < sizes.Length; i++)
    {
        var s = sizes[i] >= 256 ? 0 : sizes[i];
        w.Write((byte)s); w.Write((byte)s); w.Write((byte)0); w.Write((byte)0);
        w.Write((ushort)1); w.Write((ushort)32);
        w.Write(pngs[i].Length); w.Write(offset);
        offset += pngs[i].Length;
    }
    foreach (var png in pngs) w.Write(png);
});
thread.SetApartmentState(ApartmentState.STA);
thread.Start();
thread.Join();
Console.WriteLine($"Wrote {icoPath}");
