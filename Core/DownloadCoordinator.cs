using VidDrop.Errors;
using VidDrop.Models;

namespace VidDrop.Core;

public class DownloadCoordinator
{
    private readonly DownloadEngine _engine = new();
    private CancellationTokenSource? _cts;

    public bool IsDownloading => _cts is not null;

    public async Task StartAsync(
        DownloadOptions options,
        IProgress<DownloadProgress> progress)
    {
        if (IsDownloading)
            throw new InvalidOperationException("Download already in progress.");

        _cts = new CancellationTokenSource();
        try
        {
            await _engine.DownloadAsync(options, progress, _cts.Token);
        }
        finally
        {
            _cts.Dispose();
            _cts = null;
        }
    }

    public void Cancel()
    {
        _cts?.Cancel();
    }
}
