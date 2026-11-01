Show the notes of the latest version entry in `version.md` (project root).

Arguments: $ARGUMENTS
Format: `[type]` (optional)

Types (case-insensitive):
- `N`: Note
- `T`: To-do list
- `R`: Reminder

Process:
1. Find the latest version entry: the first `## Version: ...` section under `# Version history`. If `version.md` doesn't exist or has no version entry, tell the user to run `/version-note` first and stop.
2. Collect the notes in that entry (the `Reminder`, `Note` and `To-do list` groups added by `/add-note`, after `Summaries:`).
   - If a type is given, show only that group. If the type isn't `N`, `T` or `R`, ask the user and stop.
   - If there are no notes (or none of the given type), say so and suggest `/add-note`.
3. Show the user the version heading followed by the notes exactly as written, in the order Reminder, Note, To-do list. Don't show the summaries.
4. Don't change any files.
