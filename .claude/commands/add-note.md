Add a note to the latest version entry in `version.md` (project root).

Arguments: $ARGUMENTS
Format: `[type of note] [input note]`

Types (case-insensitive):
- `N`: Note
- `T`: To-do list
- `R`: Reminder

Process:
1. Parse the arguments:
   - The first word is the type. The rest is the note.
   - If the type is missing or isn't `N`, `T` or `R`, or the note is empty, ask the user and stop.
2. Find the latest version entry: the first `## Version: ...` section under `# Version history`. If `version.md` doesn't exist or has no version entry, tell the user to run `/version-note` first and stop.
3. Format the note by type. The group label is the type's full name (`Reminder`, `Note`, `To-do list`), not the letter:
   - `N`: the note as plain text.
     ```
     Note:
     [the note]
     ```
   - `T`: a list, each item on its own line starting with `- `. If the note contains several items (separated by new lines, `;` or `,`), split them into separate items.
     ```
     To-do list:
     - [item 1]
     - [item 2]
     ```
   - `R`: highlighted as a bold blockquote with a warning sign.
     ```
     > **⚠️ Reminder:**
     > **[the reminder]**
     ```
4. Add the note to the end of the latest version entry (after `Summaries:` and any existing notes), with a blank line before it.
   - Group notes by type, always in the order Reminder, Note, To-do list.
   - If a group of the same type already exists in this version entry, merge into it instead of adding a new group:
     - `N`: add the note as a new line in the `Note:` group.
     - `T`: add the new items to the `To-do list:` list.
     - `R`: add a new `> **[the reminder]**` line to the `Reminder` blockquote.
5. Check for similar notes: compare the new note with all existing notes in the same version entry (any type, and the summary lines too). If it shares one or more similar points with another note (same topic, overlapping task, duplicate reminder), still add it, but tell the user which existing note(s) it overlaps with and how, and suggest merging or removing the duplicate.
6. Show the user the updated version entry. Don't commit unless asked.
