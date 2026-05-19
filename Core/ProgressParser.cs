using System.Globalization;
using VidDrop.Models;

namespace VidDrop.Core;

public class ProgressParser
{
    private int _streamIndex;
    private readonly bool _isAudioOnly;

    public DownloadStage Stage { get; private set; } = DownloadStage.Preparing;

    public ProgressParser(bool isAudioOnly = false)
    {
        _isAudioOnly = isAudioOnly;
    }

    public DownloadProgress? Feed(string line)
    {
        if (line.StartsWith("[download] Destination:", StringComparison.OrdinalIgnoreCase))
        {
            _streamIndex++;
            Stage = (_isAudioOnly || _streamIndex > 1) ? DownloadStage.Audio : DownloadStage.Video;
            return new DownloadProgress { Percent = 0, Stage = Stage };
        }

        if (line.StartsWith("[Merger]", StringComparison.OrdinalIgnoreCase) ||
            line.StartsWith("[ExtractAudio]", StringComparison.OrdinalIgnoreCase) ||
            line.StartsWith("[ffmpeg]", StringComparison.OrdinalIgnoreCase))
        {
            Stage = DownloadStage.Converting;
            return new DownloadProgress { Percent = 100, Stage = Stage };
        }

        var parts = line.Split('|');
        if (parts.Length < 1) return null;

        var percentStr = parts[0].Trim().TrimEnd('%').Trim();
        if (!double.TryParse(percentStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var pct))
            return null;

        return new DownloadProgress
        {
            Percent = pct,
            Speed = parts.Length > 1 ? parts[1].Trim() : string.Empty,
            Eta = parts.Length > 2 ? parts[2].Trim() : string.Empty,
            Stage = Stage,
        };
    }
}
