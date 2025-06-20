namespace Sirstrap.Core
{
    public class FuchsiaConfiguration
    {
        public class Fuchsia
        {
            private static string GetConfigurationPath()
            {
                string configurationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Fuchsia", "Fuchsia.ini");

                Directory.CreateDirectory(Path.GetDirectoryName(configurationPath)!);

                return configurationPath;
            }

            private static void SaveConfiguration()
            {
                try
                {
                    List<string> configuration =
                    [
                        $"# Fuchsia",
                        $"ChannelName={ChannelName}",
                        $"MultiInstance={MultiInstance}",
                        $"RobloxApi={RobloxApi}",
                        $"RobloxCdnUrl={RobloxCdnUrl}",
                    ];
                    string configurationPath = GetConfigurationPath();

                    File.WriteAllLines(configurationPath, configuration);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, nameof(SaveConfiguration));
                }
            }

            public static void LoadConfiguration()
            {
                try
                {
                    string configurationPath = GetConfigurationPath();

                    if (File.Exists(configurationPath))
                    {
                        List<string> configuration = [.. File.ReadAllLines(configurationPath)];
                        List<string> foundKeys = [];

                        foreach (string line in configuration)
                        {
                            if (string.IsNullOrWhiteSpace(line)
                                || line.StartsWith('#'))
                                continue;

                            string[] parts = line.Trim().Split('=', 2);

                            if (parts.Length != 2)
                                continue;

                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            if (string.IsNullOrWhiteSpace(key)
                                || string.IsNullOrWhiteSpace(value))
                                continue;

                            foundKeys.Add(key);

                            switch (key)
                            {
                                case "ChannelName":
                                    ChannelName = value;
                                    break;
                                case "MultiInstance":
                                    MultiInstance = bool.TryParse(value, out bool multiInstance)
                                        && multiInstance;
                                    break;
                                case "RobloxApi":
                                    RobloxApi = bool.TryParse(value, out bool robloxApi)
                                        && robloxApi;
                                    break;
                                case "RobloxCdnUrl":
                                    RobloxCdnUrl = value;
                                    break;
                            }
                        }

                        if (!foundKeys.Contains("ChannelName")
                            || !foundKeys.Contains("MultiInstance")
                            || !foundKeys.Contains("RobloxApi")
                            || !foundKeys.Contains("RobloxCdnUrl"))
                            SaveConfiguration();
                    }
                    else
                        SaveConfiguration();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, nameof(LoadConfiguration));
                }
            }

            public static string ChannelName { get; set; } = "beta";

            public static bool MultiInstance { get; set; } = true;

            public static bool RobloxApi { get; set; } // = true;

            public static string RobloxCdnUrl { get; set; } = "https://setup.rbxcdn.com";
        }

        public class Roblox(HttpClient httpClient)
        {
            private const string _robloxApi = "https://clientsettingscdn.roblox.com/v1/client-version/WindowsPlayer";
            private const string _sirhurtApi = "https://sirhurt.net/status/fetch.php?exploit=SirHurt%20V5";

            private static readonly Dictionary<string, string> _binaryTypes = new(StringComparer.OrdinalIgnoreCase)
            {
                { "WindowsPlayer", "/" },
                //{ "WindowsStudio64", "/" },
                //{ "MacPlayer", "/mac/" },
                //{ "MacStudio", "/mac/" }
            };

            private async Task<string> FetchVersionHashRobloxApiAsync()
            {
                try
                {
                    using JsonDocument document = JsonDocument.Parse(await httpClient.GetStringAsync(_robloxApi));

                    if (document.RootElement.TryGetProperty("clientVersionUpload", out var version))
                        return version.GetString() ?? string.Empty;

                    return string.Empty;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, nameof(FetchVersionHashRobloxApiAsync));

                    return string.Empty;
                }
            }

            private async Task<string> FetchVersionHashSirhurtApiAsync()
            {
                try
                {
                    using JsonDocument document = JsonDocument.Parse(await httpClient.GetStringAsync(_sirhurtApi));

                    if (document.RootElement.EnumerateArray().FirstOrDefault().TryGetProperty("SirHurt V5", out var sirhurt))
                    {
                        if (sirhurt.TryGetProperty("roblox_version", out var version))
                            return version.GetString() ?? string.Empty;

                        return string.Empty;
                    }

                    return string.Empty;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, nameof(FetchVersionHashRobloxApiAsync));

                    return string.Empty;
                }
            }

            public async Task FetchVersionHashAsync()
            {
                try
                {
                    string versionHash = string.Empty;

                    if (!Fuchsia.RobloxApi)
                        versionHash = await FetchVersionHashSirhurtApiAsync();

                    if (string.IsNullOrWhiteSpace(versionHash))
                        versionHash = await FetchVersionHashRobloxApiAsync();

                    VersionHash = versionHash;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, nameof(FetchVersionHashAsync));
                }
            }

            public static void SetBinaryType(string binaryType)
            {
                try
                {
                    if (_binaryTypes.TryGetValue(binaryType, out string? value))
                    {
                        BinaryType = binaryType;
                        BlobDirectory = value;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, nameof(SetBinaryType));
                }
            }

            public static string BinaryType { get; private set; } = "WindowsPlayer";

            public static string BlobDirectory { get; private set; } = "/";

            public static string ChannelName { get; set; } = "LIVE";

            public static string LaunchUrl { get; set; } = string.Empty;

            public static string VersionHash { get; set; } = string.Empty;
        }
    }
}