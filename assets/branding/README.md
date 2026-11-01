# Branding assets

Drop the app icon master file here as **`DarkNote.svg`**.

It's the single DarkNote icon used everywhere: the `.exe`, taskbar, Alt+Tab, the main page, every note window, the Clipboard Note and the system tray (PRD 2.6).

## Tips for the SVG
- Square canvas (e.g. `viewBox="0 0 256 256"`), with the artwork filling most of it.
- Keep it simple and high-contrast: it must still read at 16 × 16 px (tray, title bar).
- It should be visible on both light and dark taskbars.
- Use plain shapes and paths (no embedded bitmaps, no external fonts: convert text to outlines).

## Generated file
`DarkNote.svg` is converted to a multi-size `src/DarkNote/Resources/Branding/DarkNote.ico` (256, 64, 48, 40, 32, 24, 20, 16 px) with the WPF-based tool in `tools/` (no ImageMagick needed). After changing the SVG, regenerate it from the repo root:

```
dotnet run tools/make-ico.cs -- assets/branding/DarkNote.svg src/DarkNote/Resources/Branding/DarkNote.ico <preview-folder>
```

The tool supports the simple shape of this SVG: `<path>` elements with root-level `stroke`, `stroke-width` and per-path `fill`.
