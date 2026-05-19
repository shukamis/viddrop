using VidDrop.Models;
using VidDrop.Process;

namespace VidDrop.Core;

public class MetadataFetcher
{
    private readonly YtDlpProcessRunner _runner = new();
    private static readonly HttpClient _http = new();

    public async Task<VideoMetadata?> FetchAsync(string url, CancellationToken ct)
    {
        try
        {
            var args = new[]
            {
                "--skip-download", "--no-playlist",
                "--print", "%(title)s",
                "--print", "%(thumbnail)s",
                url
            };

            var result = await _runner.RunAsync(args, cancellationToken: ct);
            if (!result.Success) return null;

            var lines = result.Output
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (lines.Length < 2) return null;

            var title = lines[0];
            var thumbUrl = lines[1];

            System.Drawing.Image? thumb = null;
            if (Uri.TryCreate(thumbUrl, UriKind.Absolute, out _))
            {
                var bytes = await _http.GetByteArrayAsync(thumbUrl, ct);
                using var ms = new System.IO.MemoryStream(bytes);
                thumb = System.Drawing.Image.FromStream(ms);
            }

            return new VideoMetadata { Title = title, Thumbnail = thumb };
        }
        catch
        {
            return null;
        }
    }
}
