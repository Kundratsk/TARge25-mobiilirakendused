using Plugin.Maui.Audio;

namespace TowerDefense.Services;

// Heliefektid ja taustamuusika; arvestab seadete lülitit ja helitugevust
public static class AudioService
{
    private static IAudioPlayer? _music;
    private static DateTime _lastShot = DateTime.MinValue;

    // Muusika on efektidest vaiksem, et see ei summutaks mängu
    private const double MusicVolumeFactor = 0.5;

    // ---------- Heliefektid ----------

    public static async Task PlayEffectAsync(string fileName)
    {
        if (!SettingsService.Current.SoundEnabled) return;

        try
        {
            Stream stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            IAudioPlayer player = AudioManager.Current.CreatePlayer(stream);
            player.Volume = SettingsService.Current.Volume;

            // Kui heli on lõppenud, vabastame mängija ja voo
            player.PlaybackEnded += (_, _) =>
            {
                player.Dispose();
                stream.Dispose();
            };

            player.Play();
        }
        catch
        {
            // Heli viga ei tohi mängu kokku jooksutada
        }
    }

    // Lasuhelile on piirang: mitu tuld korraga ei tee müra
    public static async Task PlayShotAsync()
    {
        DateTime now = DateTime.UtcNow;
        if ((now - _lastShot).TotalMilliseconds < 120) return;
        _lastShot = now;

        await PlayEffectAsync("shoot.wav");
    }

    // ---------- Taustamuusika ----------

    public static async Task StartMusicAsync()
    {
        if (!SettingsService.Current.SoundEnabled) return;

        try
        {
            if (_music == null)
            {
                Stream stream = await FileSystem.OpenAppPackageFileAsync("music.wav");
                _music = AudioManager.Current.CreatePlayer(stream);
                _music.Loop = true;
            }

            _music.Volume = SettingsService.Current.Volume * MusicVolumeFactor;

            if (!_music.IsPlaying)
                _music.Play();
        }
        catch
        {
            // Heli viga ei tohi mängu kokku jooksutada
        }
    }

    public static void PauseMusic()
    {
        try
        {
            if (_music is { IsPlaying: true })
                _music.Pause();
        }
        catch { }
    }

    public static void StopMusic()
    {
        try
        {
            _music?.Stop();
        }
        catch { }
    }

    // Kutsutakse siis, kui kasutaja muudab seadetes heli lülitit või helitugevust
    public static void ApplySettings()
    {
        try
        {
            if (_music == null) return;

            if (!SettingsService.Current.SoundEnabled)
            {
                _music.Pause();
                return;
            }

            _music.Volume = SettingsService.Current.Volume * MusicVolumeFactor;
        }
        catch { }
    }
}