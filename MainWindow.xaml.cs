using Microsoft.Win32;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace TextEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string? _currentFilePath;
    private bool _hasUnsavedChanges;

    public MainWindow()
    {
        InitializeComponent();
        UpdateTitle();
    }

    private void ToggleBold_Click(object sender, RoutedEventArgs e)
    {
        if (Editor.Selection.IsEmpty)
            return;

        object currentWeight = Editor.Selection.GetPropertyValue(TextElement.FontWeightProperty);
        FontWeight newWeight = currentWeight is FontWeight weight && weight == FontWeights.Bold
            ? FontWeights.Normal
            : FontWeights.Bold;

        Editor.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, newWeight);
        Editor.Focus();
    }

    private void ToggleItalic_Click(object sender, RoutedEventArgs e)
    {
        if (Editor.Selection.IsEmpty)
            return;

        object currentStyle = Editor.Selection.GetPropertyValue(TextElement.FontStyleProperty);
        FontStyle newStyle = currentStyle is FontStyle style && style == FontStyles.Italic
            ? FontStyles.Normal
            : FontStyles.Italic;

        Editor.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, newStyle);
        Editor.Focus();
    }

    private void IncreaseFont_Click(object sender, RoutedEventArgs e)
    {
        ChangeSelectedFontSize(2);
    }

    private void DecreaseFont_Click(object sender, RoutedEventArgs e)
    {
        ChangeSelectedFontSize(-2);
    }

    private void ChangeSelectedFontSize(double change)
    {
        if (Editor.Selection.IsEmpty)
            return;

        object currentValue = Editor.Selection.GetPropertyValue(TextElement.FontSizeProperty);
        double currentSize = currentValue is double size ? size : Editor.FontSize;
        double newSize = Math.Clamp(currentSize + change, 8, 96);

        Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
        Editor.Focus();
    }

    private void Editor_SelectionChanged(object sender, RoutedEventArgs e)
    {
        SelectionToolbar.Visibility = Editor.Selection.IsEmpty
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private async void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog
        {
            Title = "Open a file",
            Filter = "Rich Text Format (*.rtf)|*.rtf|Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            await LoadDocumentAsync(dialog.FileName);
            
            _currentFilePath = dialog.FileName;
            _hasUnsavedChanges = false;

            UpdateTitle();
        }
    }

    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        _hasUnsavedChanges = true;
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        string fileName = _currentFilePath is null
            ? "Untitled"
            : Path.GetFileName(_currentFilePath);

        string unsavedMarker = _hasUnsavedChanges ? "*" : "";

        Title = $"{fileName}{unsavedMarker} — Text Editor";
    }

    private async void SaveFile_Click(
        object sender,
        RoutedEventArgs e)
    {
        await SaveCurrentFileAsync();
    }

    private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            e.Handled = true;
            await SaveCurrentFileAsync();
        }
    }

    private async Task SaveCurrentFileAsync()
    {
        if (_currentFilePath is null)
        {
            await SaveAsAsync();
            return;
        }

        await SaveAsync(_currentFilePath);
    }

    private async void SaveAsFile_Click(
        object sender,
        RoutedEventArgs e)
    {
        await SaveAsAsync();
    }

    private async Task SaveAsAsync()
    {
        SaveFileDialog dialog = new SaveFileDialog
        {
            Title = "Save file as",
            Filter = "Rich Text Format (*.rtf)|*.rtf|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            DefaultExt = ".rtf",
            AddExtension = true
        };

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            await SaveAsync(dialog.FileName);
        }
    }

    private async Task SaveAsync(string filePath)
    {
        TextRange documentRange = new(Editor.Document.ContentStart, Editor.Document.ContentEnd);

        if (Path.GetExtension(filePath).Equals(".rtf", StringComparison.OrdinalIgnoreCase))
        {
            await using FileStream stream = File.Create(filePath);
            documentRange.Save(stream, DataFormats.Rtf);
        }
        else
        {
            string text = documentRange.Text;
            if (text.EndsWith("\r\n", StringComparison.Ordinal))
                text = text[..^2];

            await File.WriteAllTextAsync(filePath, text);
        }

        _currentFilePath = filePath;
        _hasUnsavedChanges = false;

        UpdateTitle();
    }

    private async Task LoadDocumentAsync(string filePath)
    {
        Editor.Document.Blocks.Clear();
        TextRange documentRange = new(Editor.Document.ContentStart, Editor.Document.ContentEnd);

        if (Path.GetExtension(filePath).Equals(".rtf", StringComparison.OrdinalIgnoreCase))
        {
            await using FileStream stream = File.OpenRead(filePath);
            documentRange.Load(stream, DataFormats.Rtf);
        }
        else
        {
            documentRange.Text = await File.ReadAllTextAsync(filePath);
        }
    }

}
