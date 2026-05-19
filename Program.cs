using Velopack;
using VidDrop.UI;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        VelopackApp.Build().Run(); // must be first — intercepts install/uninstall hooks

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
