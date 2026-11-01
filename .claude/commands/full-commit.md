Record a new version, commit, and push, in one go.

Version (if provided): $ARGUMENTS

Process:
1. Run `/version-note`, passing the version if one was provided. Follow its rules (if it asks the user something or stops, stop here too). Show the user the new entry.
2. Run `/commit`. `version.md` is now part of the changes, so don't ask about `/version-note` again. Stage all changes, including new, deleted and renamed files.
3. Run `/push` with the current branch. If the current branch is `main`, confirm with the user before pushing, as `/push` requires.
4. Tell the user the new version, the commit hash and message, and where it was pushed.
