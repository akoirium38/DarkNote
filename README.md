# Project Template

A starter template for new projects, preloaded with [Claude Code](https://claude.com/claude-code) slash commands that handle the everyday git workflow and session housekeeping.

---

## 📌 Start here: `/new-project`

**Use this command first, every time you start a new project from this template.**

```
/new-project
```

What it does:

1. Removes this template's git history.
2. Deletes the template's `version.md`, so your project starts its own version history (run `/version-note` to create it).
3. Deletes itself (`.claude/commands/new-project.md`), so it can't be run by mistake on a working project.
4. Asks you to choose an IDE (Visual Studio recommended, VS Code, or both) and a development methodology (Waterfall recommended). Then sets the project up for the IDE(s) (empty solution file, `.vscode/` config, `.gitignore` entries). The backend language isn't asked here: it's decided in `PRD.md`, and the language-specific setup is finished then. If `PRD.md` doesn't name one, Claude reminds you and recommends some options.
5. Creates `CLAUDE.md` with your choices and these rules: all output must be in English, whatever the input language; Claude must tell you which languages and technologies will be used before the development phase starts; Claude helps you create `plan.md` (a checklist per Waterfall stage: PRD, Design, Implement) when designing the stages; and in the Implement stage, functions are built one at a time, waiting for your go-ahead between each.
6. Creates `project-local-note.md` for your own local notes about the project (git-ignored, never committed).
7. Initializes a fresh git repository.
8. Adds all current files and makes the first commit.
9. Pushes to your new remote. It asks for the remote URL if none is set; the default branch is `main`.

> ⚠️ **Only run this on a fresh copy of the template.** It deletes the existing git history and `version.md`. If the folder contains anything besides `.claude/commands`, `.gitignore`, `README.md` and `version.md`, Claude will warn you and should not proceed.

### Setting up a new project

1. Clone or copy this template into a new folder:
   ```
   git clone https://github.com/akoirium38/Project-template my-new-project
   cd my-new-project
   ```
2. Create an empty repository on GitHub for the new project and copy its URL.
3. Open Claude Code in the folder and run `/new-project`. Give it the URL when asked.
4. Optionally run `/init` to expand the `CLAUDE.md` (e.g. with a "Project structure" section, which `/wrap-up` uses).

---

## Commands

All commands live in `.claude/commands/` and are run from Claude Code by typing `/<name>`.

### Git workflow

| Command | What it does | When to use it |
|---|---|---|
| `/pull` | Pulls the latest code from the remote and suggests fixes if conflicts occur. | At the start of a session, or before merging. |
| `/git-branch [name]` | Creates a branch from `main` and switches to it, using `feature/...` or `bug/...` naming. Asks for a name if none is given. | When starting a new feature or bug fix. Example: `/git-branch feature/login-page` |
| `/commit` | Summarizes your changes and commits with that summary as the message. If the changes haven't been recorded in `version.md`, it recommends running `/version-note` first. | When a piece of work is done. |
| `/full-commit [version]` | Runs `/version-note`, `/commit` and `/push` in one go on the current branch. Passes the version on to `/version-note` if given, and still confirms before pushing to `main`. | When a piece of work is done and ready to share. Example: `/full-commit` or `/full-commit 2.0.0` |
| `/push [branch]` | Pushes a branch to the remote. Asks for the branch if missing, and asks for confirmation before pushing to `main`. | To share or back up a branch. Example: `/push feature/login-page` |
| `/merge` | Safely merges the current branch into `main`: checks for uncommitted changes, updates `main`, merges, and helps resolve conflicts. | When a feature branch is finished and tested. |

### Session housekeeping

| Command | What it does | When to use it |
|---|---|---|
| `/learn-by-mistake` | Records recent mistakes and their fixes in `common_errors.md`, grouped by error category. | After fixing a tricky bug, so it isn't repeated. |
| `/version-note [version]` | Adds a new version entry (version, type of change, date, summary) to the top of `version.md`. Starts at `1.0.0`. With no version given, it bumps automatically: major for a new stage or big changes, minor for a finished feature or part of a stage, patch for small changes and hot fixes. | When you finish a stage, a feature, or a fix. Example: `/version-note` or `/version-note 2.0.0` |
| `/add-note [type] [note]` | Adds a note to the latest version entry in `version.md`. Types: `N` (Note), `T` (To-do list, each item starts with `-`), `R` (Reminder, highlighted). Notes are labeled with the full type name, and notes in the same version are grouped and merged by type, and Claude tells you if a new note overlaps with an existing one. | To record notes, to-dos or reminders for the current version. Example: `/add-note T write tests; update docs` |
| `/show-note [type]` | Shows the notes of the latest version entry in `version.md`, in the order Reminder, Note, To-do list. Give a type (`N`, `T` or `R`) to show only that group. Read-only. | To check the current reminders, notes and to-dos. Example: `/show-note` or `/show-note T` |
| `/explain [part(s)]` | Explains the newest code changes (uncommitted changes, or the latest commit if there are none), split into numbered parts by what they do: added, updated or deleted. With no part given, it lists the parts and asks which to explain; give part numbers or `all` to go straight to the explanation. Read-only. | To review code before moving on, e.g. after each function in the Implement stage. Example: `/explain` or `/explain 2 3` or `/explain all` |
| `/wrap-up` | Ends the session: logs issues and fixes (via `/learn-by-mistake`) and updates the "Project structure" section of `CLAUDE.md` if top-level files or folders changed. | At the end of every work session. |

### Project setup

| Command | What it does | When to use it |
|---|---|---|
| `/new-project` | Resets git history, deletes the template's `version.md` and itself, and pushes this template as a brand-new project. See [Start here](#-start-here-new-project). | **Once**, right after cloning the template. |

---

## Typical workflow

```
/pull                        # get the latest main
/git-branch feature/x        # start a new branch
... write code ...
/commit                      # commit with an auto-generated message
/push feature/x              # back up the branch
/merge                       # merge into main (local)
/push main                   # publish main
/wrap-up                     # log lessons and update CLAUDE.md
```

## Notes

- `/merge` only commits locally. Follow it with `/push main`.
- `/merge` stages files with `git add .` after resolving conflicts, so check `git status` for stray files first.
- `/wrap-up` expects a `CLAUDE.md` with a "Project structure" section. Run `/init` to create one if it's missing.

## Customizing

To add your own command, create a Markdown file in `.claude/commands/`. The file name becomes the command name, and the file's contents are the instructions Claude follows. Use `$ARGUMENTS` inside the file to pass in whatever you type after the command.
