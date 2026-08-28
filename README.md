# Text Editor Prototype

This repository contains an experimental desktop text editor built with C#,
.NET 10, and Windows Presentation Foundation (WPF).

The project is a prototype created to explore WPF fundamentals, including XAML
layouts, file dialogs, keyboard shortcuts, rich-text editing, and event-driven
user interfaces. It is not intended to be a production-ready text editor.

## Current features

- Open plain-text (`.txt`) and Rich Text Format (`.rtf`) files.
- Save and Save As support.
- Save the current document with `Ctrl+S`.
- Display an unsaved-changes marker in the window title.
- Apply bold, italic, and font-size changes to selected text.
- Show a contextual formatting toolbar when text is selected.
- Preserve rich-text formatting when documents are saved as `.rtf`.

## Requirements

- Windows
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Run the prototype

From the project directory, run:

```powershell
dotnet run
```

## Prototype limitations

- Plain-text files cannot preserve formatting such as bold or italic text.
- Closing a document with unsaved changes is not yet protected by a confirmation
  dialog.
- Error handling, automated tests, accessibility, and production packaging are
  still incomplete.
- The interface and feature set may change frequently while the prototype is
  being developed.
