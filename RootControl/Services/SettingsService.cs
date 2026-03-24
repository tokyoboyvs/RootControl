using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RootControl.Models;
using Windows.Storage;

namespace RootControl.Services;

public sealed class SettingsService
{
    private const string SettingsFileName = "settings.json";
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _settingsFilePath;

    public SettingsService()
    {
        _settingsFilePath = Path.Combine(
            ApplicationData.Current.LocalFolder.Path,
            SettingsFileName);
    }

    public async Task<AppSettings> LoadAsync()
    {
        if (!File.Exists(_settingsFilePath))
        {
            return new AppSettings();
        }

        await using FileStream stream = File.OpenRead(_settingsFilePath);
        AppSettings? settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream, SerializerOptions);

        return settings ?? new AppSettings();
    }

    public async Task SaveAsync(AppSettings settings)
    {
        string directory = Path.GetDirectoryName(_settingsFilePath)!;
        Directory.CreateDirectory(directory);

        await using FileStream stream = File.Create(_settingsFilePath);
        await JsonSerializer.SerializeAsync(stream, settings, SerializerOptions);
    }
}
