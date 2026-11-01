Record the current changes as a new version of the project in `version.md` (project root).

Version (if provided): $ARGUMENTS

Process:
1. Find the changes since the last version note:
   - Committed changes: `git log <last commit that touched version.md>..HEAD`. If `version.md` doesn't exist or was never committed, use the whole history.
   - Uncommitted changes: `git status` and `git diff`.
   - If there are no changes, tell the user and stop.
2. Decide the version:
   - If `version.md` doesn't exist, the first version is `1.0.0` (unless the user gave one).
   - If a version was provided, use it. It must be higher than the latest version in `version.md`; if not, ask the user.
   - Otherwise, bump the latest version based on the changes:
     - Major (`1.4.2` -> `2.0.0`): the project moves to the next stage, or has big changes.
     - Minor (`1.0.0` -> `1.1.0`): part of a stage is complete, or one or more core/main/major functions are complete, but not a whole stage yet.
     - Patch (`1.0.0` -> `1.0.1`): small changes or hot fixes.
   - If it's unclear which level fits, suggest one and ask the user.
3. Add the new entry at the top of `version.md` (newest first), below the `# Version history` heading. Create the file with that heading if it doesn't exist. Use this template:

```
## Version: [version] ([stage complete / function(s) complete / hot fix / ...]) (dd/mm/yy) 
Summaries:
- What was done, what changed, ...
```

   - The date is today's date in dd/mm/yy format.
   - Keep the summary short and readable: group related changes, skip trivial details.
4. Show the user the new entry. Don't commit unless asked.
