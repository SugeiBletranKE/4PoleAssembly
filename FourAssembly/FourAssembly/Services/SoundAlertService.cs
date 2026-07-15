namespace FourAssembly.Services;

public static class SoundAlertService
{
    private const string ScrapSoundDirectory = @"C:\ke\FourPole\SoundError";

    public static void PlayScrapSound()
    {
        try
        {
            var file = Directory
                .EnumerateFiles(ScrapSoundDirectory, "*.*")
                .FirstOrDefault(path =>
                    string.Equals(Path.GetExtension(path), ".mp3", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(Path.GetExtension(path), ".wav", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(file))
            {
                LogService.Log($"Scrap sound not found in {ScrapSoundDirectory}");
                return;
            }

            ThreadPool.QueueUserWorkItem(_ => PlayFile(file));
        }
        catch (Exception ex)
        {
            LogService.LogError("Failed to queue scrap sound", ex);
        }
    }

    private static void PlayFile(string file)
    {
        try
        {
            if (string.Equals(Path.GetExtension(file), ".wav", StringComparison.OrdinalIgnoreCase))
            {
                using var soundPlayer = new System.Media.SoundPlayer(file);
                soundPlayer.PlaySync();
                return;
            }

            var playerType = Type.GetTypeFromProgID("WMPlayer.OCX");
            if (playerType == null)
            {
                LogService.LogError("Windows Media Player COM is not available. Cannot play MP3 scrap sound.");
                return;
            }

            dynamic mediaPlayer = Activator.CreateInstance(playerType)!;
            mediaPlayer.URL = file;
            mediaPlayer.controls.play();
        }
        catch (Exception ex)
        {
            LogService.LogError($"Failed to play scrap sound: {file}", ex);
        }
    }
}
