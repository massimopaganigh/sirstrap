using Serilog;

namespace Sirstrap.Core
{
    // Refactored
    public static class DirectoriesManager
    {
        /// <summary>
        /// Creates all required application directories if they don't already exist.
        /// </summary>
        /// <remarks>
        /// This method creates the following directories:
        /// <list type="bullet">
        ///   <item><description>Sirstrap directory - main application folder</description></item>
        ///   <item><description>Cache directory - for temporary files</description></item>
        ///   <item><description>Logs directory - for application logs</description></item>
        ///   <item><description>Versions directory - for different Roblox versions</description></item>
        /// </list>
        /// </remarks>
        public static void CreateDirectories()
        {
            Directory.CreateDirectory(Directories.SirstrapDirectory);
            Directory.CreateDirectory(Directories.CacheDirectory);
            Directory.CreateDirectory(Directories.LogsDirectory);
            Directory.CreateDirectory(Directories.VersionsDirectory);
        }

        /// <summary>
        /// Deletes the cache directory and all its contents.
        /// </summary>
        /// <remarks>
        /// If the cache directory does not exist, this method does nothing.
        /// The method uses recursive deletion to remove all files and subdirectories.
        /// </remarks>
        public static void DeleteCacheDirectory()
        {
            if (Directory.Exists(Directories.CacheDirectory))
            {
                Directory.Delete(Directories.CacheDirectory, true);

                Log.Information("[*] Cache directory deleted.");
            }
        }

        /// <summary>
        /// Deletes the installation directory for a specific Roblox version and all its contents.
        /// </summary>
        /// <param name="version">The Roblox version string identifying the installation to delete.</param>
        /// <remarks>
        /// If the installation directory for the specified version does not exist, this method does nothing.
        /// The method uses recursive deletion to remove all files and subdirectories within the installation folder.
        /// </remarks>
        public static void DeleteInstallDirectory(string version)
        {
            var installDirectory = GetInstallDirectory(version);

            if (Directory.Exists(installDirectory))
            {
                Directory.Delete(installDirectory, true);

                Log.Information("[*] Installation directory for version {0} deleted.", version);
            }
        }

        /// <summary>
        /// Gets the installation directory path for a specific Roblox version.
        /// </summary>
        /// <param name="version">The Roblox version string.</param>
        /// <returns>
        /// A fully qualified path to the directory where the specified Roblox version
        /// should be installed.
        /// </returns>
        public static string GetInstallDirectory(string version)
        {
            return Path.Combine(Directories.VersionsDirectory, version);
        }
    }
}