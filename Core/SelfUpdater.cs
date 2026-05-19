using System.Diagnostics;
using VidDrop.Process;

namespace VidDrop.Core;

public static class SelfUpdater
{
    public static void RunInBackground()
    {
        Task.Run(async () =>
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ToolLocator.GetYtDlpPath(),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                psi.ArgumentList.Add("-U");

                using var p = new System.Diagnostics.Process { StartInfo = psi };
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                await p.WaitForExitAsync();
            }
            catch { }
        });
    }
}
