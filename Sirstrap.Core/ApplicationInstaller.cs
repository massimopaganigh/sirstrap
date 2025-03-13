using Serilog;
using System.IO.Compression;

namespace Sirstrap.Core
{
    public static class ApplicationInstaller
    {
        public static void Install(DownloadConfiguration downloadConfiguration)
        {
            try
            {
                var targetPath = PathManager.GetVersionInstallPath(downloadConfiguration.Version);
                var zipPath = downloadConfiguration.GetOutputFileName();

                PrepareDirectory(targetPath);

                try
                {
                    using var archive = ZipFile.OpenRead(zipPath);

                    foreach (var entry in archive.Entries)
                    {
                        if (!string.IsNullOrEmpty(entry.Name))
                        {

                            var targetSubPath = Path.GetFullPath(Path.Combine(targetPath, entry.FullName));

                            Directory.CreateDirectory(Path.GetDirectoryName(targetSubPath) ?? string.Empty);

                            entry.ExtractToFile(targetSubPath, true);
                        }
                    }
                }
                finally
                {
                    DeleteFile(zipPath);

                    Log.Information("[*] Cleaning completed, {0} deleted.", zipPath);
                }

                Log.Information("[*] Extraction completed, version {0} extracted to {1}.", downloadConfiguration.Version, targetPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error during extraction, installation failed: {0}.", ex.Message);

                throw; // Re-throw the exception to halt execution
            }
        }

        private static void PrepareDirectory(string directory)
        {
            try
            {
                var parentDirectory = Directory.GetParent(directory)!.FullName;

                if (Directory.Exists(parentDirectory))
                {
                    try // try-catch goofing around with directory deletion
                    {
                        Directory.Delete(parentDirectory, true);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        foreach (var file in Directory.GetFiles(parentDirectory, "*", SearchOption.AllDirectories))
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

                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error during preparation: {0}.", ex.Message);

                throw; // Re-throw the exception to halt execution
            }
        }

        private static void DeleteFile(string filePath, int attempts = 3)
        {
            try
            {
                foreach (var attempt in Enumerable.Range(1, attempts))
                {
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        return;
                    }
                    catch (Exception ex) when (attempt < attempts)
                    {
                        Log.Error(ex, "[!] Error during cleaning: {0}. Trying again in {1}ms.", ex.Message, 100 * attempt);
                        Thread.Sleep(100 * attempt);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error during cleaning: {0}.", ex.Message);

                throw; // Re-throw the exception to halt execution
            }
        }
    }
}