using System.Diagnostics;
using System.Text;

namespace VidDrop.Process;

public class YtDlpProcessRunner
{
    public async Task<ProcessResult> RunAsync(
        IEnumerable<string> arguments,
        Action<string>? onOutputLine = null,
        CancellationToken cancellationToken = default)
    {
        var outputLines = new List<string>();
        var errorLines = new List<string>();

        var psi = new ProcessStartInfo
        {
            FileName = ToolLocator.GetYtDlpPath(),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };

        foreach (var arg in arguments)
            psi.ArgumentList.Add(arg);

        using var process = new System.Diagnostics.Process { StartInfo = psi, EnableRaisingEvents = true };

        var outputDone = new TaskCompletionSource<bool>();
        var errorDone = new TaskCompletionSource<bool>();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is null) { outputDone.TrySetResult(true); return; }
            outputLines.Add(e.Data);
            onOutputLine?.Invoke(e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is null) { errorDone.TrySetResult(true); return; }
            errorLines.Add(e.Data);
            onOutputLine?.Invoke(e.Data);
        };

        bool wasCancelled = false;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var reg = cancellationToken.Register(() =>
        {
            wasCancelled = true;
            try { process.Kill(entireProcessTree: true); } catch { }
        });

        await Task.WhenAll(
            process.WaitForExitAsync(CancellationToken.None),
            outputDone.Task,
            errorDone.Task);

        return new ProcessResult
        {
            ExitCode = wasCancelled ? -1 : process.ExitCode,
            Output = string.Join(Environment.NewLine, outputLines),
            Error = string.Join(Environment.NewLine, errorLines),
            WasCancelled = wasCancelled
        };
    }
}
