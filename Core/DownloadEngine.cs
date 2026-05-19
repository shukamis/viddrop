using VidDrop.Errors;
using VidDrop.Models;
using VidDrop.Process;

namespace VidDrop.Core;

public class DownloadEngine
{
    private readonly YtDlpProcessRunner _runner = new();

    private static readonly string OutputFolder =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads");

    public async Task DownloadAsync(
        DownloadOptions options,
        IProgress<DownloadProgress> progress,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(OutputFolder);

        var toolsDir = ToolLocator.GetToolsDir();
        var tempDir = Path.Combine(Path.GetTempPath(), "viddrop-work");
        Directory.CreateDirectory(tempDir);

        var parser = new ProgressParser(options.Format == MediaFormat.Mp3);

        var args = new List<string>
        {
            "--ffmpeg-location", toolsDir,
            "--restrict-filenames",
            "--paths", $"home:{OutputFolder}",
            "--paths", $"temp:{tempDir}",
            "-f", options.GetFormatSelector(),
            "-o", "%(title)s.%(ext)s",
            "--progress-template", "%(progress._percent_str)s|%(progress._speed_str)s|%(progress._eta_str)s",
            "--newline",
            "--no-playlist",
        };

        if (options.Format == MediaFormat.Mp3)
        {
            args.AddRange([
                "-x",
                "--audio-format", "mp3",
                "--audio-quality", $"{(int)options.Bitrate}k",
                "--embed-thumbnail",
            ]);
        }
        else
        {
            args.Add("--merge-output-format");
            args.Add("mp4");
        }

        args.Add(options.Url);

        try
        {
            var result = await _runner.RunAsync(args, line =>
            {
                var p = parser.Feed(line);
                if (p is not null)
                    progress.Report(p);
            }, cancellationToken);

            if (result.WasCancelled)
                throw new DownloadException(ErrorCategory.Cancelled, "Download cancelado.");

            if (!result.Success)
                throw ErrorClassifier.Classify(result.Error + result.Output);
        }
        finally
        {
            if (cancellationToken.IsCancellationRequested)
                CleanTempDir(tempDir);
        }
    }

    private static void CleanTempDir(string dir)
    {
        try { if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true); } catch { }
    }
}
