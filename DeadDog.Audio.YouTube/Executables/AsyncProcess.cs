using System.Diagnostics;
using System.Threading.Tasks;

namespace DeadDog.Audio.YouTube.Executables
{
    internal static class AsyncProcess
    {
        public static Task RunProcessAsync(this ProcessStartInfo startInfo)
        {
            var tcs = new TaskCompletionSource<bool>();
            startInfo.RedirectStandardError = true;

            var process = new Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };
            process.Exited += (s, e) =>
            {
                if (s is Process p && p.ExitCode != 0)
                {
                    var error = p.StandardError.ReadToEnd();
                }
                process.Dispose();
                tcs.SetResult(true);
            };

            process.Start();

            return tcs.Task;
        }
    }
}
