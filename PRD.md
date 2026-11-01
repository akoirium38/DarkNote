#  DarkNote — Desktop Sticky Notes App for Windows

---

## PART 1: PROJECT OVERVIEW

### 1.1 Project Overview

Build a lightweight desktop sticky-notes app for Windows that lets the user jot quick notes, keep them floating above other windows, and collect copied text in one place, without saving manually.

The app allows:

- A **main page** that lists all notes with their name, color code and icon, with search and sort
- Each note opens as its **own small window** with a colored banner, a black body with white text, rich text formatting, pin-on-top and adjustable transparency
- A special **Clipboard Note** that captures text and file paths copied from anywhere while it is open
- **Autosave** and restoring the desktop layout (window position, size, pinned state) after a restart

### 1.2 Target Users

- **User**: a single person using the app on their own Windows computer. There are no accounts, roles or login; all data is stored locally on the computer.

---

## PART 2: FUNCTIONAL REQUIREMENTS

### 2.1 Main Page

#### 2.1.1 View Note List

**Actor:** User  
**Requirement:** App is running

**Displayed information (per note):**

- Note name
- Color code (color of the note's banner)
- Icon (from the preset icon set, see 2.2.5)

**Functions:**

- Click a note in the list to open its note window (or bring it to the front if it is already open)
- The Clipboard Note (2.3) is always shown at the top of the list, with its dark gray color and `copy` icon

#### 2.1.2 Search Notes

**Actor:** User  
**Requirement:** At least one note exists

**Flow:**

1. User types in the search box on the main page
2. The list filters as the user types, showing only notes whose **name** contains the search text
3. Clearing the search box shows all notes again

**Rules:**

- Searches note names only (not note content)
- Case-insensitive

#### 2.1.3 Sort Notes

**Actor:** User  
**Requirement:** At least one note exists

**Sort options (4 separate options):**

- **Last edited** (default): most recently edited first
- **Color**: rainbow order — red, orange, yellow, green, blue, purple, pink, white, gray
- **Name**: A–Z
- **Icon**: follows the fixed order of the icon preset list (2.2.5), not the picker's recently-used order
- The Clipboard Note always stays at the top, whatever the sort option

**Rules:**

- The chosen sort option is remembered in `settings.json` (2.4.3)

#### 2.1.4 Create Note from Main Page

**Actor:** User  
**Requirement:** App is running

**Flow:**

1. User clicks "New note" on the main page
2. App creates a new note with the default settings (see 2.2.1) and opens its window

#### 2.1.5 Delete Note

**Actor:** User  
**Requirement:** At least one note exists

**Flow:**

1. User clicks "Delete" on a note (in the list or in the note window)
2. App asks for confirmation
3. App closes the note window (if open), deletes the note's JSON file and removes it from the list

**Rules:**

- Empty notes are deleted automatically when closed (see 2.2.2)
- The Clipboard Note cannot be deleted; its Delete button is hidden (individual clipboard items can still be deleted, see 2.3.3)

---

### 2.2 Note Window

**Note window layout (3 parts):**

- **Banner** (top): note name, color code, icon, and buttons (pin, transparency, delete, and the **controls** button)
- **Body**: the black background area
- **Text**: the note content shown on the body

**Expanded banner (controls):**

- Clicking the **controls** button extends the banner downward to show the note's controls; clicking it again collapses the banner
- Controls shown in the expanded banner:
  - Formatting buttons (2.2.3), including Undo / Redo and Change case
  - Font and font size (2.2.4)
  - Color and icon pickers (2.2.5)
  - Opacity slider (2.2.7)

#### 2.2.1 Create New Note

**Actor:** User  
**Requirement:** App is running

**Flow:**

1. User presses `Ctrl + N` while a note window is in focus (or uses 2.1.4)
2. App creates a new note with the defaults below and opens it in a new window
3. The new note appears in the main page list

**Default settings for a new note:**

- Font: Segoe UI
- Font size: Medium
- Color: yellow
- Icon: `notebook`
- Pin on top: off
- Opacity: 100%

#### 2.2.2 Edit Note Content &amp; Name

**Actor:** User  
**Requirement:** Note window is open

**Flow:**

1. User types in the note body
2. The note name is taken automatically from the first 3 words of the note's first line, followed by "..." when the line has more than 3 words
3. User can rename the note manually by double-clicking the name in the banner, or pressing `F2` while the note is in focus; `Enter` saves the new name, `Esc` cancels. Once renamed, the name no longer follows the first line

**Validation Rules:**

- Note name: while the first line is empty, the note is named "New"
- Empty note: if a note with no content is closed, it is deleted automatically (no confirmation); the Clipboard Note is never auto-deleted
- Note name: maximum 30 characters for all names (automatic and manual); manual renaming cannot exceed 30 characters, and an automatic name longer than 30 characters is cut off with "..."

#### 2.2.3 Text Formatting

**Actor:** User  
**Requirement:** Note window is open

**Available formatting:**

- Bold
- Italic
- Underline
- Strikethrough
- Bulleted list
- Numbered list
- Checkbox / to-do list (each item can be checked and unchecked)
- Undo / Redo buttons
- Change case button (`Shift + F3`): cycles the selected text (or the word at the cursor) through UPPERCASE → lowercase → Capitalize Each Word, like Microsoft Word

**Display:**

- Note body: black background, white text

#### 2.2.4 Font &amp; Font Size

**Actor:** User  
**Requirement:** Note window is open

**Font (chosen per note, 3 choices):**

- Segoe UI (default)
- JetBrains Mono
- Caveat

**Font size (3 presets):**

- Small
- Medium (default)
- Large

**Rules:**

- Font and font size apply to the whole note
- Caveat looks smaller than the other fonts at the same point size, so its presets are scaled up (about 20–30%) to look the same size as the other fonts

#### 2.2.5 Color &amp; Icon

**Actor:** User  
**Requirement:** Note window is open

**Color palette (banner color):**


| Color                           | Hex (draft) |
| ------------------------------- | ----------- |
| Yellow (default)                | `#FACC15`   |
| Red                             | `#EF4444`   |
| Blue                            | `#3B82F6`   |
| White                           | `#F5F5F5`   |
| Green                           | `#22C55E`   |
| Purple                          | `#A855F7`   |
| Pink                            | `#EC4899`   |
| Orange                          | `#F97316`   |
| Gray                            | `#9CA3AF`   |
| Dark gray (Clipboard Note only) | `#1A1A1A`   |


**Icon preset (Lucide):**
`info`, `calendar`, `lightbulb`, `lock`, `database`, `folder`, `gift`, `bot-message-square`, `code-xml`, `notebook`, `cat`, `mail-open`, `fish`, `book`, `bookmark`, `book-text`, `list`, `link`, `play`, `store`, `languages`

**Rules:**

- Dark gray is reserved for the Clipboard Note and cannot be chosen for regular notes
- Text, icons and buttons on the banner are black on yellow, white, gray, orange, green and pink, and white on red, blue and purple, so they stay readable
- The icon picker shows the most recently chosen icons first

#### 2.2.6 Pin on Top

**Actor:** User  
**Requirement:** Note window is open

**Flow:**

1. User clicks the pin button on the note's banner
2. When on, the note window stays above all other windows
3. Clicking the pin button again turns it off

#### 2.2.7 Transparent Body

**Actor:** User  
**Requirement:** Note window is open

**Flow:**

1. User sets the opacity with the slider in the expanded banner
2. The transparency button on the banner switches the body between 100% and the last slider value (80% if the slider has never been used)

**Validation Rules:**

- Opacity range: 20%–100% (the minimum keeps the note from becoming invisible)

**Rules:**

- Opacity only affects the **body** (background)
- The **banner** and the **text** always stay fully visible
- The note's **border** always stays visible, so the note's outline can be seen at any opacity

---

### 2.3 Clipboard Note

**General rules:**

- Always shown at the top of the main page list, whatever the sort option
- Banner buttons: pin on top, transparency, and the controls button only (no formatting, font, color, icon, rename or delete)
- Its expanded banner shows only the opacity slider and the history size option (2.3.4)
- Its name is fixed: "Clipboard"
- Its history is saved and survives app restarts (2.4.3)

#### 2.3.1 Capture Copied Text

**Actor:** User  
**Requirement:** Clipboard Note window is open

**Flow:**

1. User copies text in any app
2. App adds the copied text as a new item at the top of the Clipboard Note

**Rules:**

- Only captures while the Clipboard Note window is open
- Captures text and file paths: copying a file in File Explorer saves its full path as a text item (e.g. `D:\code\notes\PRD.md`); copying several files at once saves one item with one path per line
- Images are ignored
- Skips a copy if it is identical to the **latest** item only (kept simple)
- Skips passwords copied from password managers

#### 2.3.2 View Clipboard History in Pages

**Actor:** User  
**Requirement:** Clipboard Note window is open

**Display:**

- Items shown newest first
- 3 items per page
- Items longer than 200 characters are cut off with "...show more"
- Older copies move to the next pages
- Banner color: dark gray `#1A1A1A`, so it stays visible against the black body; icon: Lucide `copy`

#### 2.3.3 Clipboard Item Actions

**Actor:** User  
**Requirement:** Clipboard Note has at least one item

**Each item has:**

- **Copy** button: copies the text to the clipboard. The item is deleted, and the copy itself is captured again as a new item on top (so a re-used item moves to the top)
- **Delete** button: removes the item
- **Edit**: click on the text to edit it

#### 2.3.4 History Size Option

**Actor:** User  
**Requirement:** Clipboard Note window is open

**Flow:**

1. User clicks the controls button on the Clipboard Note's banner to expand it
2. User selects the history size: 5, 10 or 20 items (default 10)
3. When the history is full, the oldest item is removed

---

### 2.4 Data &amp; Persistence

#### 2.4.1 Autosave

**Actor:** System  
**Requirement:** A note is open

**Rules:**

- Every change (content, name, formatting, color, icon, font, pin, opacity) is saved automatically
- No save button
- Saves only the file of the note that changed

#### 2.4.2 Remember Window State

**Actor:** System  
**Requirement:** App is restarted

**Flow:**

1. On closing the app, each note's window state is already saved (2.4.1)
2. On starting the app, notes that were open reopen at the same position and size, with the same pinned state and opacity

**Saved per note:**

- Window position and size
- Pinned state
- Opacity
- Whether the note was open

#### 2.4.3 Storage Structure

**Actor:** System

**Structure:**

```
<app data folder>/
├─ settings.json      (app-wide settings: sort choice, main window position, Clipboard Note history size, recently used icons)
├─ clipboard.json     (Clipboard Note: saved items, window position/size, pinned state, opacity, open state)
└─ notes/
   ├─ <note-id>.json  (one note: content, name, whether the name was set manually, color, icon, font, font size, window position/size, pinned state, opacity, open state, last edited)
   └─ ...
```

**Rules:**

- One JSON file per note, so notes saving at the same time never overwrite each other and a damaged file affects only one note
- Write to a temporary file first, then replace the old file, to avoid damage if the app crashes during a save

---

### 2.5 Keyboard Shortcuts


| Shortcut                             | Where                     | Action                                                                    |
| ------------------------------------ | ------------------------- | ------------------------------------------------------------------------- |
| `Ctrl + N`                           | A note window is in focus | Create a new note (2.2.1)                                                 |
| `F2`                                 | A note window is in focus | Rename the note (2.2.2)                                                   |
| `Ctrl + B` / `Ctrl + I` / `Ctrl + U` | Note body                 | Bold / Italic / Underline                                                 |
| `Ctrl + Z`                           | Note body                 | Undo                                                                      |
| `Ctrl + Y`                           | Note body                 | Redo                                                                      |
| `Shift + F3`                         | Note body                 | Cycle case of selected text: UPPERCASE → lowercase → Capitalize Each Word |


**Button tooltips:**

- Hovering over any button shows its name, plus its shortcut if it has one, e.g. "Undo (Ctrl + Z)", "Bold (Ctrl + B)", "Pin on top"

---

### 2.6 System Tray Icon &amp; App Icon

**Actor:** User  
**Requirement:** App is running

**App icon:**

- One DarkNote icon is shared everywhere: the `.exe`, taskbar, Alt+Tab, the main page, every note window, the Clipboard Note and the system tray
- Source: an SVG master in `assets/branding/DarkNote.svg`, converted to a multi-size `.ico` (16, 20, 24, 32, 40, 48, 64, 256 px)

**Tray icon:**

- Shown in the Windows notification area while the app is running; tooltip "DarkNote"
- **Left-click:** shows the main page (restores it and brings it to the front)
- **Right-click menu:** Open DarkNote, New note, Clipboard Note, Exit

**Flow:**

1. User closes the main page with its X button
2. The main page is hidden to the tray; the app and all open notes keep running
3. User clicks the tray icon to show the main page again, or chooses **Exit** from the tray menu to close the app

**Rules:**

- The first time the main page is hidden, a notification says "DarkNote is still running in the system tray"
- **Exit** saves everything and closes all windows; notes that were open reopen at the next start (2.4.2)
- Windows sign-out or shutdown behaves like Exit
- Only one copy of the app can run: starting DarkNote again shows the existing main page

---

## PART 3: DEPENDENCY TABLE

The table below shows dependencies between features. The feature in the "Feature" column depends on the features listed in the "Depends on" column.


| No. | Feature                         | Code  | Depends on          | Notes                                                     | Status        |
| --- | ------------------------------- | ----- | ------------------- | --------------------------------------------------------- | ------------- |
| 1   | Storage Structure               | 2.4.3 | -                   | Foundation: file layout for notes and settings            | ⬜ Not started |
| 2   | Create New Note                 | 2.2.1 | 2.4.3               | Needs storage to save the new note                        | ⬜ Not started |
| 3   | Edit Note Content &amp; Name    | 2.2.2 | 2.2.1               | Needs a note window                                       | ⬜ Not started |
| 4   | Autosave                        | 2.4.1 | 2.4.3, 2.2.2        | Saves edits automatically                                 | ⬜ Not started |
| 5   | View Note List                  | 2.1.1 | 2.4.3               | Reads all note files                                      | ⬜ Not started |
| 6   | Create Note from Main Page      | 2.1.4 | 2.1.1, 2.2.1        | "New note" button                                         | ⬜ Not started |
| 7   | Delete Note                     | 2.1.5 | 2.1.1               | From the list or the note window, with confirmation       | ⬜ Not started |
| 8   | Text Formatting                 | 2.2.3 | 2.2.2               | Bold, italic, underline, strikethrough, lists, checkboxes | ⬜ Not started |
| 9   | Font &amp; Font Size            | 2.2.4 | 2.2.2               | 3 fonts, 3 size presets                                   | ⬜ Not started |
| 10  | Color &amp; Icon                | 2.2.5 | 2.2.1               | Palette + 21 Lucide icons, recently-used picker           | ⬜ Not started |
| 11  | Pin on Top                      | 2.2.6 | 2.2.1               | Window always on top                                      | ⬜ Not started |
| 12  | Transparent Body                | 2.2.7 | 2.2.1               | Opacity slider                                            | ⬜ Not started |
| 13  | Remember Window State           | 2.4.2 | 2.4.1, 2.2.6, 2.2.7 | Restores layout after restart                             | ⬜ Not started |
| 14  | Search Notes                    | 2.1.2 | 2.1.1               | Search by name                                            | ⬜ Not started |
| 15  | Sort Notes                      | 2.1.3 | 2.1.1, 2.2.5        | Needs color and icon data                                 | ⬜ Not started |
| 16  | Capture Copied Text             | 2.3.1 | 2.2.1, 2.4.3        | Clipboard listener                                        | ⬜ Not started |
| 17  | View Clipboard History in Pages | 2.3.2 | 2.3.1               | 3 items per page, 200-character cut-off                   | ⬜ Not started |
| 18  | Clipboard Item Actions          | 2.3.3 | 2.3.2               | Copy, delete, edit                                        | ⬜ Not started |
| 19  | History Size Option             | 2.3.4 | 2.3.2               | 5 / 10 / 20 items                                         | ⬜ Not started |
| 20  | Keyboard Shortcuts              | 2.5   | 2.2.1, 2.2.3        | `Ctrl + N`, undo/redo and formatting shortcuts            | ⬜ Not started |
| 21  | System Tray Icon &amp; App Icon | 2.6   | 2.1.1, 2.4.2        | Shared app icon, hide main page to tray, single instance  | ⬜ Not started |


### Notes on implementation order:

**Phase 1 - Foundation (Highest priority):**

- 2.4.3 Storage Structure
- 2.2.1 Create New Note
- 2.2.2 Edit Note Content &amp; Name
- 2.4.1 Autosave

**Phase 2 - Main Page:**

- 2.1.1 View Note List
- 2.1.4 Create Note from Main Page
- 2.1.5 Delete Note

**Phase 3 - Note Features:**

- 2.2.3 Text Formatting
- 2.2.4 Font &amp; Font Size
- 2.2.5 Color &amp; Icon
- 2.2.6 Pin on Top
- 2.2.7 Transparent Body
- 2.5 Keyboard Shortcuts

**Phase 4 - Persistence &amp; Organization:**

- 2.4.2 Remember Window State
- 2.1.2 Search Notes
- 2.1.3 Sort Notes
- 2.6 System Tray Icon &amp; App Icon

**Phase 5 - Clipboard Note (Lowest priority):**

- 2.3.1 Capture Copied Text
- 2.3.2 View Clipboard History in Pages
- 2.3.3 Clipboard Item Actions
- 2.3.4 History Size Option

## PART 4: TECH STACK

#### Platform

- **Target OS:** Windows only
- **IDE:** Visual Studio
- **Development style:** vibe-coding (AI-assisted), feature by feature following Part 3

#### App Framework

- **Language:** C#
- **UI Framework:** WPF
- **Runtime:** .NET 10 (LTS)
- **Architecture:** MVVM with CommunityToolkit.Mvvm

#### Key Building Blocks

- **Note windows:** `WindowStyle="None"` + `AllowsTransparency="True"` (frameless, per-pixel transparency so only the body fades), `Topmost` for pin on top, `WindowChrome` for resizing
- **Rich text editor:** WPF `RichTextBox` (`FlowDocument`)
  - Built in: bold, italic, underline (`Ctrl + B / I / U`), bullet and numbered lists, undo/redo (`Ctrl + Z / Y`)
  - Custom: strikethrough, checkbox / to-do list, change case (`Shift + F3`)
- **Clipboard listener:** Windows `AddClipboardFormatListener` (`WM_CLIPBOARDUPDATE`); reads text and file drop lists (file paths); skips copies marked by password managers (`ExcludeClipboardContentFromMonitorProcessing`)
- **System tray icon:** H.NotifyIcon.Wpf (`TaskbarIcon` with a XAML context menu); chosen over WinForms `NotifyIcon` to avoid `UseWindowsForms` name clashes and keep the dark-themed menu
- **Single instance:** a named `Mutex`; a second launch signals the running app to show its main page, then exits
- **Opening folders / apps / links (future special notes):** `Process.Start` with `UseShellExecute`

#### UI Assets

- **App icon:** `assets/branding/DarkNote.svg` (master) → multi-size `DarkNote.ico`, set as `<ApplicationIcon>`; windows don't set their own `Icon`, so they all use the app icon
- **Icons:** Lucide (SVG path data stored as XAML resources)
- **Fonts:** Segoe UI (built into Windows), JetBrains Mono, Caveat (both SIL Open Font License, bundled with the app as resources)

#### Data Storage

- **Format:** JSON with `System.Text.Json` (one file per note + `settings.json` + `clipboard.json`)
- **Note content:** saved as `FlowDocument` XAML inside the note's JSON file
- **Location:** `%LOCALAPPDATA%\<app name>\`

#### Testing

- **Unit tests:** xUnit v3 (Microsoft Testing Platform), project `tests/DarkNote.Tests`
- **WPF tests:** Xunit.StaFact (`[WpfFact]` / `[WpfTheory]` run on an STA thread, needed for `RichTextBox` / `FlowDocument`)
- **Fakes:** NSubstitute
- **Run:** `dotnet test --solution DarkNote.sln` or Visual Studio Test Explorer

#### Other Tools

- **Version Control:** Git + GitHub (commit after every working feature)

---

## PART 5: OPEN QUESTIONS &amp; FUTURE IDEAS

### Open Questions

- Confirm the exact hex values of the color palette (draft in 2.2.5)
- Checkbox / to-do list implementation in `RichTextBox` (custom; see Part 4)
- App name (current name is a work-in-progress)

### Future Updates

The project is open for future updates, especially more **special notes** like the Clipboard Note. Examples:

- **Link Note**: saves links as clickable items; clicking a link opens it in the user's browser
- **Quick-Access Paths Note**: saves paths to folders (mainly) and apps as clickable items; clicking a folder opens it in File Explorer, clicking an app launches it

Other future features:

- **PIN lock**: locks the whole app with a PIN
  - Lock with `Ctrl + Alt + L` or a lock button
  - When locked, all notes only show black (content, names hidden) until the PIN is entered

**Design guideline:** build the Clipboard Note as the first of a general "special note" type (fixed name, fixed color and icon, its own controls and its own data file), so new special notes can be added later without changing regular notes.

### Ideas to Consider (Not Yet In Scope)

- Collapse a note to just its banner
- Opacity returns to full on hover
- Trash: keep deleted notes for a while so they can be restored