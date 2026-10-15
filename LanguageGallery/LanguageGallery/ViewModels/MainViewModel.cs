using System.Collections.ObjectModel;
using LanguageGallery.Models;

namespace LanguageGallery.ViewModels;

public class MainViewModel
{
    public ObservableCollection<LanguageCard> Cards { get; } = new()
    {
        new("CSharp",     "C#",         "csharp.png"),
        new("Python",     "Python",     "python.png"),
        new("JavaScript", "JavaScript", "javascript.png"),
        new("Java",       "Java",       "java.png"),
        new("Cpp",        "C++",        "cpp.png"),
    };

    public void RefreshCards()
    {
        foreach (var card in Cards)
            card.Refresh();
    }
}