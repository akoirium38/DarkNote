# Branding assets

Drop the app icon master file here as **`DarkNote.svg`**.

It's the single DarkNote icon used everywhere: the `.exe`, taskbar, Alt+Tab, the main page, every note window, the Clipboard Note and the system tray (PRD 2.6).

## Tips for the SVG
- Square canvas (e.g. `viewBox="0 0 256 256"`), with the artwork filling most of it.
- Keep it simple and high-contrast: it must still read at 16 × 16 px (tray, title bar).
- It should be visible on both light and dark taskbars.
- Use plain shapes and paths (no embedded bitmaps, no external fonts: convert text to outlines).

## Generated file
In Stage 3 setup (task 3.0.6), `DarkNote.svg` is converted to a multi-size `DarkNote.ico` (16, 20, 24, 32, 40, 48, 64, 256 px), for example:

```
magick DarkNote.svg -background none -define icon:auto-resize=256,64,48,40,32,24,20,16 DarkNote.ico
```
