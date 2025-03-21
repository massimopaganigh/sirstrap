using Serilog;
using System.Diagnostics;

namespace Sirstrap.Core
{
    // Refactored
    public static class RobloxLauncher
    {
        /// <summary>
        /// Launches a specific version of Roblox Player executable.
        /// </summary>
        /// <param name="downloadConfiguration">Configuration containing version information and optional launch URL.</param>
        /// <param name="waitForExit">If true, captures a singleton and waits for the process to exit. Default is false.</param>
        /// <returns>True if the launch was successful; otherwise, false.</returns>
        public static bool Launch(DownloadConfiguration downloadConfiguration, bool waitForExit = false)
        {
            var launchPath = Path.Combine(DirectoriesManager.GetInstallDirectory(downloadConfiguration.Version), "RobloxPlayerBeta.exe");

            if (File.Exists(launchPath))
            {
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = launchPath,
                    WorkingDirectory = Path.GetDirectoryName(launchPath),
                    UseShellExecute = true
                };

                if (!string.IsNullOrEmpty(downloadConfiguration.LaunchUrl))
                {
                    processStartInfo.Arguments = downloadConfiguration.LaunchUrl;

                    Log.Information("[*] Launching {0} with URL: {1}.", launchPath, downloadConfiguration.LaunchUrl);
                }
                else
                {
                    Log.Information("[*] Launching {0}.", launchPath);
                }

                var capturedSingleton = waitForExit && SingletonManager.CaptureSingleton();
                var process = Process.Start(processStartInfo);

                if (capturedSingleton && process != null)
                {
                    Task.Run(() => WaitForProcessExit(process));
                }

                return true;
            }
            else
            {
                Log.Error("[!] Error during launching. Roblox executable not found at {0}.", launchPath);

                return false;
            }
        }

        /// <summary>
        /// Monitors a Roblox process and releases the singleton when the process exits.
        /// </summary>
        /// <param name="process">The Roblox process to monitor.</param>
        /// <remarks>
        /// This method runs in a background task when launching with waitForExit=true.
        /// It checks periodically if the process has exited or if no Roblox processes exist,
        /// then releases the application singleton.
        /// </remarks>
        private static void WaitForProcessExit(Process process)
        {
            while (SingletonManager.HasCapturedSingleton && !process.HasExited && Process.GetProcessesByName("RobloxPlayerBeta").Length > 0)
            {
                Thread.Sleep(100);
            }

            SingletonManager.ReleaseSingleton();
        }
    }
}