using Velopack;
using Velopack.Sources;

namespace VidDrop.Core;

public static class UpdateChecker
{
    private const string GitHubRepoUrl = "https://github.com/shukamis/viddrop";

    private static UpdateManager? _mgr;
    private static VelopackAsset? _pending;

    public static event EventHandler? UpdateReady;

    public static void RunInBackground()
    {
        _ = Task.Run(CheckAsync);
    }

    private static async Task CheckAsync()
    {
        try
        {
            _mgr = new UpdateManager(new GithubSource(GitHubRepoUrl, null, false));
            var info = await _mgr.CheckForUpdatesAsync();
            if (info == null) return;

            await _mgr.DownloadUpdatesAsync(info);
            _pending = info.TargetFullRelease;
            UpdateReady?.Invoke(null, EventArgs.Empty);
        }
        catch
        {
            // Never crash the app because of an update check failure
        }
    }

    public static void ApplyOnExit()
    {
        if (_mgr != null && _pending != null)
            _mgr.WaitExitThenApplyUpdates(_pending);
    }
}
