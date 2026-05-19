using VidDrop.Errors;

namespace VidDrop.Process;

public static class ToolLocator
{
    private static readonly string ToolsDir =
        System.IO.Path.Combine(AppContext.BaseDirectory, "Tools");

    public static string GetYtDlpPath()
    {
        var path = System.IO.Path.Combine(ToolsDir, "yt-dlp.exe");
        if (!System.IO.File.Exists(path))
            throw new DownloadException(ErrorCategory.ToolMissing,
                "yt-dlp.exe não encontrado.");
        return path;
    }

    public static string GetToolsDir()
    {
        if (!System.IO.File.Exists(System.IO.Path.Combine(ToolsDir, "ffmpeg.exe")))
            throw new DownloadException(ErrorCategory.ToolMissing,
                "ffmpeg.exe não encontrado.");
        return ToolsDir;
    }

    public static string GetFfmpegPath()
    {
        var path = System.IO.Path.Combine(ToolsDir, "ffmpeg.exe");
        if (!System.IO.File.Exists(path))
            throw new DownloadException(ErrorCategory.ToolMissing,
                "ffmpeg.exe não encontrado.");
        return path;
    }

    public static void VerifyAll()
    {
        GetYtDlpPath();
        GetFfmpegPath();
    }
}
