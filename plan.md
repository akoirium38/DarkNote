# Plan & progress

Mark `[x]` as soon as each task is done. PRD codes (e.g. PRD 2.4.3) refer to sections in `PRD.md`.

### Stage 1 — PRD
- [x] 1.1 Write `PRD.md`

### Stage 2 — Design
Each workflow goes in `docs/workflows/<PRD code>-<name>.md` as a Mermaid flowchart (happy path + edge cases).

**Phase 1 — Foundation**
- [ ] 2.1 Workflow: Storage Structure (PRD 2.4.3)
- [ ] 2.2 Workflow: Create New Note (PRD 2.2.1)
- [ ] 2.3 Workflow: Edit Note Content & Name (PRD 2.2.2)
- [ ] 2.4 Workflow: Autosave (PRD 2.4.1)

**Phase 2 — Main Page**
- [ ] 2.5 Workflow: View Note List (PRD 2.1.1)
- [ ] 2.6 Workflow: Create Note from Main Page (PRD 2.1.4)
- [ ] 2.7 Workflow: Delete Note (PRD 2.1.5)

**Phase 3 — Note Features**
- [ ] 2.8 Workflow: Text Formatting (PRD 2.2.3)
- [ ] 2.9 Workflow: Font & Font Size (PRD 2.2.4)
- [ ] 2.10 Workflow: Color & Icon (PRD 2.2.5)
- [ ] 2.11 Workflow: Pin on Top (PRD 2.2.6)
- [ ] 2.12 Workflow: Transparent Body (PRD 2.2.7)
- [ ] 2.13 Workflow: Keyboard Shortcuts (PRD 2.5)

**Phase 4 — Persistence & Organization**
- [ ] 2.14 Workflow: Remember Window State (PRD 2.4.2)
- [ ] 2.15 Workflow: Search Notes (PRD 2.1.2)
- [ ] 2.16 Workflow: Sort Notes (PRD 2.1.3)

**Phase 5 — Clipboard Note**
- [ ] 2.17 Workflow: Capture Copied Text (PRD 2.3.1)
- [ ] 2.18 Workflow: View Clipboard History in Pages (PRD 2.3.2)
- [ ] 2.19 Workflow: Clipboard Item Actions (PRD 2.3.3)
- [ ] 2.20 Workflow: History Size Option (PRD 2.3.4)

**Data & review**
- [ ] 2.21 Data design `docs/data-design.md` (JSON schemas for `settings.json`, `clipboard.json`, `notes/<note-id>.json`)
- [ ] 2.22 Cross-check workflows ↔ data design, user approves design

### Stage 3 — Implement
For each function: branch → read workflow → write unit tests → user checks tests → implement (Context7 for latest syntax) → run tests, fix until all pass → user checks → merge into `main`.

**Setup**
- [ ] 3.0.1 Confirm languages, technologies, libraries and test framework with the user
- [ ] 3.0.2 Create the WPF project (.NET 10) and add it to `DarkNote.sln`
- [ ] 3.0.3 Create the unit test project and add it to `DarkNote.sln`
- [ ] 3.0.4 Add NuGet packages (CommunityToolkit.Mvvm, test libraries)
- [ ] 3.0.5 Bundle fonts (JetBrains Mono, Caveat) and Lucide icons as resources
- [ ] 3.0.6 Add C# entries to `.gitignore` (`bin/`, `obj/`, `TestResults/`)

**Functions**
- [ ] 3.1 Storage Structure (2.1)
- [ ] 3.2 Create New Note (2.2)
- [ ] 3.3 Edit Note Content & Name (2.3)
- [ ] 3.4 Autosave (2.4)
- [ ] 3.5 View Note List (2.5)
- [ ] 3.6 Create Note from Main Page (2.6)
- [ ] 3.7 Delete Note (2.7)
- [ ] 3.8 Text Formatting (2.8)
- [ ] 3.9 Font & Font Size (2.9)
- [ ] 3.10 Color & Icon (2.10)
- [ ] 3.11 Pin on Top (2.11)
- [ ] 3.12 Transparent Body (2.12)
- [ ] 3.13 Keyboard Shortcuts (2.13)
- [ ] 3.14 Remember Window State (2.14)
- [ ] 3.15 Search Notes (2.15)
- [ ] 3.16 Sort Notes (2.16)
- [ ] 3.17 Capture Copied Text (2.17)
- [ ] 3.18 View Clipboard History in Pages (2.18)
- [ ] 3.19 Clipboard Item Actions (2.19)
- [ ] 3.20 History Size Option (2.20)

### Stage 4 — Update & new functions
Starts when Stage 3 is complete. Add tasks here as updates and new functions are planned (e.g. PRD Part 5: Link Note, Quick-Access Paths Note, PIN lock).
