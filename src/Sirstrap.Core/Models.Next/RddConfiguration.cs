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

        #region ROOTS
        public Dictionary<string, string> Roots { get; set; } = [];

        public Dictionary<string, string> PlayerRoots { get; set; } = new(StringComparer.OrdinalIgnoreCase)
        {
            { "RobloxApp.zip", string.Empty },
            { "redist.zip", string.Empty },
            { "shaders.zip", "shaders/" },
            { "ssl.zip", "ssl/" },
            { "WebView2.zip", string.Empty },
            { "WebView2RuntimeInstaller.zip", "WebView2RuntimeInstaller/" },
            { "content-avatar.zip", "content/avatar/" },
            { "content-configs.zip", "content/configs/" },
            { "content-fonts.zip", "content/fonts/" },
            { "content-sky.zip", "content/sky/" },
            { "content-sounds.zip", "content/sounds/" },
            { "content-textures2.zip", "content/textures/" },
            { "content-models.zip", "content/models/" },
            { "content-platform-fonts.zip", "PlatformContent/pc/fonts/" },
            { "content-platform-dictionaries.zip", "PlatformContent/pc/shared_compression_dictionaries/" },
            { "content-terrain.zip", "PlatformContent/pc/terrain/" },
            { "content-textures3.zip", "PlatformContent/pc/textures/" },
            { "extracontent-luapackages.zip", "ExtraContent/LuaPackages/" },
            { "extracontent-translations.zip", "ExtraContent/translations/" },
            { "extracontent-models.zip", "ExtraContent/models/" },
            { "extracontent-textures.zip", "ExtraContent/textures/" },
            { "extracontent-places.zip", "ExtraContent/places/" }
        };

        public Dictionary<string, string> StudioRoots { get; set; } = new(StringComparer.OrdinalIgnoreCase)
        {
            { "RobloxStudio.zip", string.Empty },
            { "RibbonConfig.zip", "RibbonConfig/" },
            { "redist.zip", string.Empty },
            { "Libraries.zip", string.Empty },
            { "LibrariesQt5.zip", string.Empty },
            { "WebView2.zip", string.Empty },
            { "WebView2RuntimeInstaller.zip", string.Empty },
            { "shaders.zip", "shaders/" },
            { "ssl.zip", "ssl/" },
            { "Qml.zip", "Qml/" },
            { "Plugins.zip", "Plugins/" },
            { "StudioFonts.zip", "StudioFonts/" },
            { "BuiltInPlugins.zip", "BuiltInPlugins/" },
            { "ApplicationConfig.zip", "ApplicationConfig/" },
            { "BuiltInStandalonePlugins.zip", "BuiltInStandalonePlugins/" },
            { "content-qt_translations.zip", "content/qt_translations/" },
            { "content-sky.zip", "content/sky/" },
            { "content-fonts.zip", "content/fonts/" },
            { "content-avatar.zip", "content/avatar/" },
            { "content-models.zip", "content/models/" },
            { "content-sounds.zip", "content/sounds/" },
            { "content-configs.zip", "content/configs/" },
            { "content-api-docs.zip", "content/api_docs/" },
            { "content-textures2.zip", "content/textures/" },
            { "content-studio_svg_textures.zip", "content/studio_svg_textures/" },
            { "content-platform-fonts.zip", "PlatformContent/pc/fonts/" },
            { "content-platform-dictionaries.zip", "PlatformContent/pc/shared_compression_dictionaries/" },
            { "content-terrain.zip", "PlatformContent/pc/terrain/" },
            { "content-textures3.zip", "PlatformContent/pc/textures/" },
            { "extracontent-translations.zip", "ExtraContent/translations/" },
            { "extracontent-luapackages.zip", "ExtraContent/LuaPackages/" },
            { "extracontent-textures.zip", "ExtraContent/textures/" },
            { "extracontent-scripts.zip", "ExtraContent/scripts/" },
            { "extracontent-models.zip", "ExtraContent/models/" }
        };
        #endregion

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