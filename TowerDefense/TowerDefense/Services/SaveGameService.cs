using System.Text.Json;
using TowerDefense.Models;

namespace TowerDefense.Services;

// Pooleli jäänud mängu salvestamine ja laadimine (eraldi fail seadetest)
public static class SaveGameService
{
    private static readonly string FilePath =
        Path.Combine(FileSystem.AppDataDirectory, "savegame.json");

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch
        {
            // Salvestamise viga ei tohi mängu kokku jooksutada
        }
    }

    // Tagastab null, kui salvestust pole või fail on katki
    public static SaveData? Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return null;

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<SaveData>(json);
        }
        catch
        {
            return null;
        }
    }

    public static void Delete()
    {
        try
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
        catch
        {
            // Kustutamise viga pole kriitiline
        }
    }
}