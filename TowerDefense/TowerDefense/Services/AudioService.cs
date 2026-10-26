
using Plugin.Maui.Audio;

namespace TowerDefense.Services;

public static class AudioService
{
    private static IAudioPlayer? _music;
    private static Stream? _musicStream;
    private static DateTime _lastShot = DateTime.MinValue;

    private const double MusicVolumeFactor = 0.5;

    public static async Task PlayEffectAsync(string fileName)
    {
        if (!SettingsService.Current.SoundEnabled)
            return;

        try
        {
            Stream stream =
                await FileSystem.OpenAppPackageFileAsync(fileName);

            IAudioPlayer player;

            try
            {
                player = AudioManager.Current.CreatePlayer(stream);
            }
            catch
            {
                stream.Dispose();
                throw;
            }

            player.Volume = SettingsService.Current.Volume;

            player.PlaybackEnded += (_, _) =>
            {
                try
                {
                    stream.Dispose();
                }
                catch
                {
                    // Voog võib olla juba suletud.
                }
            };

            player.Play();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Heliefekti viga ({fileName}): {ex.Message}");
        }
    }

    public static async Task PlayShotAsync()
    {
        DateTime now = DateTime.UtcNow;

        if ((now - _lastShot).TotalMilliseconds < 120)
            return;

        _lastShot = now;

        await PlayEffectAsync("shoot.wav");
    }

    public static async Task StartMusicAsync()
    {
        if (!SettingsService.Current.SoundEnabled)
            return;

        try
        {
            if (_music == null)
            {
                _musicStream =
                    await FileSystem.OpenAppPackageFileAsync("music.wav");

                try
                {
                    _music = AudioManager.Current.CreatePlayer(_musicStream);
                    _music.Loop = true;
                }
                catch
                {
                    _musicStream.Dispose();
                    _musicStream = null;
                    throw;
                }
            }

            _music.Volume =
                SettingsService.Current.Volume * MusicVolumeFactor;

            if (!_music.IsPlaying)
                _music.Play();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Taustamuusika viga: {ex.Message}");
        }
    }

    public static void PauseMusic()
    {
        try
        {
            if (_music?.IsPlaying == true)
                _music.Pause();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Muusika peatamise viga: {ex.Message}");
        }
    }

    public static void StopMusic()
    {
        try
        {
            _music?.Stop();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Muusika peatamise viga: {ex.Message}");
        }
    }

    public static void ApplySettings()
    {
        try
        {
            if (_music == null)
                return;

            if (!SettingsService.Current.SoundEnabled)
            {
                _music.Pause();
                return;
            }

            _music.Volume =
                SettingsService.Current.Volume * MusicVolumeFactor;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Heliseadete rakendamise viga: {ex.Message}");
        }
    }
}
