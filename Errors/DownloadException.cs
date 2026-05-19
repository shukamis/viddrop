namespace VidDrop.Errors;

public class DownloadException : Exception
{
    public ErrorCategory Category { get; }

    public DownloadException(ErrorCategory category, string message, Exception? inner = null)
        : base(message, inner)
    {
        Category = category;
    }
}
