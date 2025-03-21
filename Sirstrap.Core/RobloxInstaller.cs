using Serilog;
using System.IO.Compression;

namespace Sirstrap.Core
{
    // Refactored
    public static class RobloxInstaller
    {
        /// <summary>
        /// Installs a specific version of Roblox by extracting files from a downloaded zip archive.
        /// </summary>
        /// <param name="downloadConfiguration">Configuration containing version information and download details.</param>
        /// <exception cref="Exception">Thrown when installation encounters an error.</exception>
        public static void Install(DownloadConfiguration downloadConfiguration)
        {
            try
            {
                var installDirectory = DirectoriesManager.GetInstallDirectory(downloadConfiguration.Version);
                var zipPath = downloadConfiguration.GetOutputFileName();

                Prepare(installDirectory);

                try
                {
                    foreach (var entry in ZipFile.OpenRead(zipPath).Entries)
                    {
                        if (!string.IsNullOrEmpty(entry.Name))
                        {
                            var installChildrenDirectory = Path.GetFullPath(Path.Combine(installDirectory, entry.FullName));

                            Directory.CreateDirectory(Path.GetDirectoryName(installChildrenDirectory)!);

                            entry.ExtractToFile(installChildrenDirectory, true);
                        }
                    }
                }
                finally
                {
                    Clean(zipPath);

                    Log.Information("[*] Cleaning completed, {0} deleted.", zipPath);
                }

                Log.Information("[*] Installation completed, version {0} extracted to {1}.", downloadConfiguration.Version, installDirectory);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error during installation: {0}.", ex.Message);

                throw; // Re-throw the exception to halt execution
            }
        }

        /// <summary>
        /// Prepares the installation directory by removing any existing files and creating a clean directory structure.
        /// </summary>
        /// <param name="installDirectory">The target directory where Roblox will be installed.</param>
        /// <exception cref="Exception">Thrown when directory preparation encounters an error.</exception>
        private static void Prepare(string installDirectory)
        {
            try
            {
                var installParentDirectory = Directory.GetParent(installDirectory)?.FullName;

                if (Directory.Exists(installParentDirectory))
                {
                    try // try-catch goofing around with directory deletion
                    {
                        Directory.Delete(installParentDirectory, true);
                    }
                    catch
                    {
                        foreach (var file in Directory.GetFiles(installParentDirectory, "*", SearchOption.AllDirectories))
                        {
                            try // try-catch goofing around with file deletion
                            {
                                File.Delete(file);
                            }
                            catch
                            {
                                // Ignore any errors during file deletion
                            }
                        }
                    }

                    Directory.CreateDirectory(installDirectory);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error during preparation: {0}.", ex.Message);

                throw; // Re-throw the exception to halt execution
            }
        }

        /// <summary>
        /// Removes the downloaded zip file after installation is complete.
        /// Includes retry logic to handle potential file access issues.
        /// </summary>
        /// <param name="zipPath">The path to the zip file to be deleted.</param>
        /// <param name="attempts">The maximum number of deletion attempts (default: 5).</param>
        /// <exception cref="Exception">Thrown when file deletion fails after all attempts.</exception>
        private static void Clean(string zipPath, int attempts = 5)
        {
            try
            {
                foreach (var attempt in Enumerable.Range(1, attempts))
                {
                    try
                    {
                        if (File.Exists(zipPath))
                        {
                            File.Delete(zipPath);
                        }

                        return;
                    }
                    catch (Exception ex) when (attempt < attempts)
                    {
                        Log.Error(ex, "[!] Error during cleaning: {0}. Trying again in {1}ms.", ex.Message, 100 * attempt);
                        Thread.Sleep(100 * attempt);
                    }
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error during cleaning: {0}.", ex.Message);

                throw; // Re-throw the exception to halt execution
            }
        }
    }
}