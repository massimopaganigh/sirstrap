using Serilog;
using System.Text.Json;

namespace Sirstrap.Core.Models.Next
{
    public class SrddConfiguration(HttpClient httpClient, SirstrapConfiguration sirstrapConfiguration) : RddConfiguration
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly SirstrapConfiguration _sirstrapConfiguration = sirstrapConfiguration;

        public async Task SetVersionHashAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (BinaryType.Equals("WindowsPlayer"))
                    if (_sirstrapConfiguration.FetchVersionFromSirHurt)
                        VersionHash = await FetchVersionFromSirHurtAsync(cancellationToken).ConfigureAwait(false);
                    else
                        VersionHash = await FetchVersionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(SetVersionHashAsync));
            }
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