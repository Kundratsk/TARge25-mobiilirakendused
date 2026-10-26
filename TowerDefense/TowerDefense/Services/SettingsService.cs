using System.Text.Json;
using TowerDefense.Models;

namespace TowerDefense.Services;

// Staatiline klass: kogu rakenduses on üks ja sama seadete objekt
public static class SettingsService
{
    // AppDataDirectory on rakenduse enda privaatne kaust igal platvormil
    private static readonly string FilePath =
        Path.Combine(FileSystem.AppDataDirectory, "settings.json");

    private static AppSettings? _current;

    // ??= tähendab: kui _current on tühi, lae see failist; muidu kasuta olemasolevat
    public static AppSettings Current => _current ??= Load();

    private static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch
        {
            // Kui fail on katki, alustame vaikeseadetega
        }

        return new AppSettings();
    }

    public static void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(
                Current,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch
        {
            // Salvestamise viga ei tohi mängu kokku jooksutada
        }
    }
}