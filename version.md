# Version history

## Version: 2.1.0 (stage 3 setup + function 3.1 Storage Structure complete) (02/11/26)
Summaries:
- Stage 3 started. Stack confirmed: C# / WPF on .NET 10, CommunityToolkit.Mvvm, H.NotifyIcon.Wpf (tray), xUnit v3 + Xunit.StaFact + NSubstitute (tests). Recorded in PRD Part 4.
- Setup (3.0.1–3.0.7): `src/DarkNote` WPF app and `tests/DarkNote.Tests` added to `DarkNote.sln`; bundled JetBrains Mono and Caveat fonts and 22 Lucide icons as resources; multi-size `DarkNote.ico` generated from the branding SVG and set as the app icon; asset tools in `tools/`; C# entries in `.gitignore`.
- Function 3.1 Storage Structure (PRD 2.4.3): data models, `StorageService` (load everything on start, safe save via `.tmp` + replace), `DataNormalizer` (fixes missing/invalid values), lenient JSON enum converters, file log in `logs\darknote.log`. Damaged files are kept as `.corrupt` and skipped; if the data folder can't be used, the app shows an error and exits.
- 104 unit tests, all passing (happy path and edge cases 1–10 of workflow 2.4.3).
- `docs/data-design.md`: added the `logs\` folder, `.corrupt` naming, what counts as a damaged file, `WindowBounds` class name.

## Version: 2.0.0 (stage 2 design complete) (02/11/26)
Summaries:
- Stage 2 complete, design approved by the user.
- Added 21 workflow files in `docs/workflows/` (one per PRD feature), each with Mermaid flowcharts for the happy path and edge cases, plus an edge-case table. All diagrams checked with the Mermaid parser.
- Added `docs/data-design.md`: JSON layout of `settings.json`, `clipboard.json` and `notes/<note-id>.json` (fields, defaults, validation, examples).
- New feature in scope: PRD 2.6 System Tray Icon & App Icon (shared app icon, main page hides to the tray, Exit from the tray menu, single instance). Removed from "Ideas to Consider".
- Added the app icon master `assets/branding/DarkNote.svg` (yellow-filled book, readable on light and dark backgrounds).
- `plan.md`: Stage 2 ticked; tray icon added (2.17 / 3.17), app icon setup task 3.0.6, later tasks renumbered.
- `/push` command: with no branch given, pushes `main` instead of asking.

## Version: 1.0.0 (project start) (01/11/26)
Summaries:
- Set up DarkNote from the project template: Visual Studio solution (`DarkNote.sln`), Waterfall methodology, `.gitignore` for Visual Studio, git-ignored `project-local-note.md`.
- Stage 1 complete: added `PRD.md` (desktop sticky-notes app for Windows; C#, WPF, .NET 10, MVVM).
- `CLAUDE.md`: project rules and the Waterfall flow for Stages 1–4 (workflow files per function, unit tests checked by the user before running, one git branch per function, merge after tests pass and user approval).
- Added `plan.md`: step-by-step checklist for Stage 2 (20 workflows, data design, cross-check) and Stage 3 (setup + 20 functions).
