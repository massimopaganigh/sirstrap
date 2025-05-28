namespace Sirstrap.Core.Models.Next
{
    public class RddConfiguration()
    {
        public string BinaryType { get; set; } = "WindowsPlayer";

        public string ChannelName { get; set; } = "LIVE";

        public string VersionHash { get; set; } = string.Empty;

        public bool CompressOutputZip { get; set; } = false;

        public int ZipCompressionLevel { get; set; } = 5;

        public string BlobDirectory { get; set; } = "/";

        public string LaunchUrl { get; set; } = string.Empty;

        #region DIRECTORIES
        public string SirstrapCacheSirstrapDirectory { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Sirstrap\\Cache\\Sirstrap";

        public string SirstrapCacheRobloxDirectory { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Sirstrap\\Cache\\Roblox";

        public string SirstrapVersionsDirectory { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Sirstrap\\Versions";
        #endregion

        #region PATHS
        public string SirstrapConfigurationPath { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Sirstrap\\Configuration.ini";
        #endregion
    }
}