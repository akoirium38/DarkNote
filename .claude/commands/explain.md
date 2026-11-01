Explain the newest code changes (added, implemented, updated and/or deleted), split into parts for easy understanding.

Arguments: $ARGUMENTS
Format: `[part(s)]` (optional): part numbers (e.g. `1`, `1 3`), or `all`.

Process:
1. Find the newest changes:
   - Uncommitted changes: `git status` and `git diff` (staged and unstaged), including new and deleted files.
   - If there are none, use the latest commit (`git show HEAD`).
   - If there's nothing to explain, tell the user and stop.
2. Split the changes into parts by what they do (e.g. one function, one feature, one workflow, a config change), not file by file. Number each part.
3. If no part was given, show the list of parts, each with:
   - A short title.
   - Its status: Added / Updated / Deleted.
   - The files involved.
   Then ask the user which part(s) to explain, or `all`, and stop until they answer.
4. Explain the chosen part(s), one section per part:
   - What it does and why it's there (or, for deleted code, what it did and what replaces it, if anything).
   - How it works, step by step, quoting short code snippets with `file_path:line_number`.
   - How it connects to the rest of the project (what calls it, what it calls, data and database tables used).
   - Anything worth watching out for (edge cases, assumptions, possible issues).
   Keep it plain and readable; explain terms the user may not know.
5. After explaining, offer to go deeper into any part or explain the remaining parts.
6. Don't change any files.
