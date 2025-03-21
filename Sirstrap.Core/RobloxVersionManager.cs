using Serilog;
using System.Text.Json;

namespace Sirstrap.Core
{
    // Refactored
    public class RobloxVersionManager(HttpClient httpClient)
    {
        private readonly HttpClientExtension _httpClientExtension = new(httpClient);

        private const string SIRHURT_API = "https://sirhurt.net/status/fetch.php?exploit=SirHurt%20V5";
        private const string ROBLOX_API = "https://clientsettingscdn.roblox.com/v1/client-version/WindowsPlayer";

        /// <summary>
        /// Retrieves the current Roblox version asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
        /// The task result contains the Roblox version as a string if successful, or <c>null</c> if an error occurred.
        /// </returns>
        public async Task<string?> GetVersionAsync()
        {
            try
            {
                Log.Information("[*] Getting Roblox version...");

                return await GetVersionFromSirHurtAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error getting Roblox version: {0}", ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Attempts to retrieve the Roblox version from the SirHurt API.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
        /// The task result contains the Roblox version as a string if successful.
        /// </returns>
        /// <remarks>
        /// If retrieval from the SirHurt API fails, this method falls back to 
        /// <see cref="GetVersionFromRobloxAsync"/> to attempt retrieval from the official Roblox API.
        /// </remarks>
        private async Task<string?> GetVersionFromSirHurtAsync()
        {
            try
            {
                var jsonDocument = await GetJsonDocumentAsync(SIRHURT_API);

                if (jsonDocument != null && jsonDocument.RootElement.EnumerateArray().FirstOrDefault().TryGetProperty("SirHurt V5", out var sirhurt) && sirhurt.TryGetProperty("roblox_version", out var version))
                {
                    Log.Information("[*] Roblox version: {0}", version.GetString());

                    return version.GetString();
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error getting Roblox version from SirHurt API: {0}. Trying again with Roblox API.", ex.Message);

                return await GetVersionFromRobloxAsync(); // Fallback
            }
        }

        /// <summary>
        /// Attempts to retrieve the Roblox version directly from the official Roblox API.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
        /// The task result contains the Roblox version as a string if successful, or <c>null</c> if an error occurred.
        /// </returns>
        /// <remarks>
        /// This method is used as a fallback when retrieval from the SirHurt API fails.
        /// </remarks>
        private async Task<string?> GetVersionFromRobloxAsync()
        {
            try
            {
                var jsonDocument = await GetJsonDocumentAsync(ROBLOX_API);

                if (jsonDocument != null && jsonDocument.RootElement.TryGetProperty("clientVersionUpload", out var version))
                {
                    Log.Information("[*] Roblox version: {0}", version.GetString());

                    return version.GetString();
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error getting Roblox version from Roblox API: {0}.", ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Helper method that retrieves and parses a JSON document from the specified URI.
        /// </summary>
        /// <param name="requestUri">The URI to send the HTTP GET request to.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
        /// The task result contains the parsed JSON document if successful, or <c>null</c> if an error occurred.
        /// </returns>
        private async Task<JsonDocument?> GetJsonDocumentAsync(string requestUri)
        {
            try
            {
                return JsonDocument.Parse(await _httpClientExtension.GetStringSafeAsync(requestUri));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error getting JSON document from {0}: {1}.", requestUri, ex.Message);

                return null;
            }
        }
    }
}