namespace Sirstrap.Core
{
    // Refactored
    public static class Directories
    {
        public static string SirstrapDirectory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Sirstrap");

        public static string CacheDirectory { get; set; } = Path.Combine(SirstrapDirectory, "Cache");

        public static string LogsDirectory { get; set; } = Path.Combine(SirstrapDirectory, "Logs");

        public static string VersionsDirectory { get; set; } = Path.Combine(SirstrapDirectory, "Versions");
    }
}