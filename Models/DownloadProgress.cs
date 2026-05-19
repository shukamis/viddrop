namespace VidDrop.Models;

public enum DownloadStage { Preparing, Video, Audio, Converting }

public class DownloadProgress
{
    public double Percent { get; init; }
    public string Speed { get; init; } = string.Empty;
    public string Eta { get; init; } = string.Empty;
    public DownloadStage Stage { get; init; } = DownloadStage.Preparing;
}
