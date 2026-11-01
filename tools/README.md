# Tools

Small helper scripts used to generate app assets. They are not part of the app build.

| Tool | What it does | Run (from repo root) |
|---|---|---|
| `make-ico.cs` | Renders `assets/branding/DarkNote.svg` into the multi-size app icon `DarkNote.ico` (uses WPF, .NET 10 file-based app) | `dotnet run tools/make-ico.cs -- <in.svg> <out.ico> <preview-folder>` |
| `lucide_to_xaml.py` | Converts Lucide SVG icons (from `lucide-static`) into `Geometry` resources in `src/DarkNote/Resources/Icons/LucideIcons.xaml` | `python -I tools/lucide_to_xaml.py <svg-folder> src/DarkNote/Resources/Icons/LucideIcons.xaml` |

Icons in `LucideIcons.xaml` come from `lucide-static` 1.66.0 (`https://unpkg.com/lucide-static@1.66.0/icons/<name>.svg`). To add one, download its SVG into the folder with the others and rerun the converter.
