namespace VidDrop.Process;

public class ProcessResult
{
    public int ExitCode { get; init; }
    public string Output { get; init; } = string.Empty;
    public string Error { get; init; } = string.Empty;
    public bool WasCancelled { get; init; }
    public bool Success => ExitCode == 0 && !WasCancelled;
}
