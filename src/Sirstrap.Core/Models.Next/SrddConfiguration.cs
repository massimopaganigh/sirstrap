namespace Sirstrap.Core.Models.Next
{
    public class SrddConfiguration(HttpClient httpClient, SirstrapConfiguration sirstrapConfiguration) : RddConfiguration, ISrddConfiguration
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly SirstrapConfiguration _sirstrapConfiguration = sirstrapConfiguration;

        public async Task SetFromArgumentsAsync(string[] arguments, CancellationToken cancellationToken)
        {
            try
            {
                if (arguments == null
                    || arguments.Length == 0)
                    throw new ArgumentNullException(nameof(arguments));

                LaunchUrl = arguments.FirstOrDefault(x => !x.StartsWith("--"), string.Empty);
                Roots = BinaryType.Equals("WindowsPlayer")
                    ? PlayerRoots
                    : StudioRoots;

                if (string.IsNullOrEmpty(VersionHash))
                    await SetVersionHashAsync(cancellationToken).ConfigureAwait(false);

                CreateDirectories();
                CreateFiles();
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(SetFromArgumentsAsync));
            }
        }

        private async Task SetVersionHashAsync(CancellationToken cancellationToken)
        {
            if (BinaryType.Equals("WindowsPlayer"))
                if (_sirstrapConfiguration.FetchVersionFromSirHurt)
                    VersionHash = await FetchVersionFromSirHurtAsync(cancellationToken).ConfigureAwait(false);
                else
                    VersionHash = await FetchVersionAsync(cancellationToken).ConfigureAwait(false);
        }

        private void CreateDirectories()
        {
            if (Directory.Exists(SirstrapCacheSirstrapDirectory))
                Directory.CreateDirectory(SirstrapCacheSirstrapDirectory);
            else
            {
                Directory.Delete(SirstrapCacheSirstrapDirectory, true);
                Directory.CreateDirectory(SirstrapCacheSirstrapDirectory);
            }

            if (!Directory.Exists(SirstrapCacheRobloxDirectory))
                Directory.CreateDirectory(SirstrapCacheRobloxDirectory);
            else
            {
                Directory.Delete(SirstrapCacheRobloxDirectory, true);
                Directory.CreateDirectory(SirstrapCacheRobloxDirectory);
            }

            if (!Directory.Exists(SirstrapVersionsDirectory))
                Directory.CreateDirectory(SirstrapVersionsDirectory);
        }

        private void CreateFiles()
        {
            if (!File.Exists(SirstrapConfigurationPath))
                File.Create(SirstrapConfigurationPath).Dispose();
        }

        private async Task<string> FetchVersionAsync(CancellationToken cancellationToken)
        {
            JsonDocument jsonDocument = await GetJsonDocumentAsync(GlobalConstants.ROBLOX_URI, cancellationToken).ConfigureAwait(false);

            if (jsonDocument != null)
                if (jsonDocument.RootElement.TryGetProperty("clientVersionUpload", out JsonElement clientVersionUpload))
                    return clientVersionUpload.ToString();

            return string.Empty;
        }

        private async Task<string> FetchVersionFromSirHurtAsync(CancellationToken cancellationToken)
        {
            JsonDocument jsonDocument = await GetJsonDocumentAsync(GlobalConstants.SIRHURT_URI, cancellationToken).ConfigureAwait(false);

            if (jsonDocument != null)
                if (jsonDocument.RootElement.EnumerateArray().FirstOrDefault().TryGetProperty("SirHurt V5", out JsonElement sirHurt))
                    if (sirHurt.TryGetProperty("roblox_version", out JsonElement clientVersionUpload))
                        return clientVersionUpload.ToString();

            return string.Empty;
        }

        private async Task<JsonDocument> GetJsonDocumentAsync(string uri, CancellationToken cancellationToken) => JsonDocument.Parse(await _httpClient.GetStringAsync(uri, cancellationToken).ConfigureAwait(false));
    }
}