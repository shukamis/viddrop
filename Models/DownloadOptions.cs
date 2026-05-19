namespace VidDrop.Models;

public enum Mp3Bitrate { K128 = 128, K192 = 192, K320 = 320 }

public class DownloadOptions
{
    public required string Url { get; init; }
    public QualityLevel Quality { get; init; } = QualityLevel.Best;
    public MediaFormat Format { get; init; } = MediaFormat.Mp4;
    public Mp3Bitrate Bitrate { get; init; } = Mp3Bitrate.K192;

    public string GetFormatSelector() => Format == MediaFormat.Mp3
        ? "bestaudio[ext=m4a]/bestaudio"
        : Quality switch
        {
            QualityLevel.Best  => "bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio/best",
            QualityLevel.P1080 => "bestvideo[height<=1080][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=1080]+bestaudio/best[height<=1080]",
            QualityLevel.P720  => "bestvideo[height<=720][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=720]+bestaudio/best[height<=720]",
            QualityLevel.P480  => "bestvideo[height<=480][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=480]+bestaudio/best[height<=480]",
            QualityLevel.P360  => "bestvideo[height<=360][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=360]+bestaudio/best[height<=360]",
            _                  => "bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio/best"
        };
}
