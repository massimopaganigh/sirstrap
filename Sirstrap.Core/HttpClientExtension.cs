using Serilog;

namespace Sirstrap.Core
{
    // Refactored
    public class HttpClientExtension(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        /// <summary>
        /// Sends a GET request to the specified URI and returns the response body as a byte array.
        /// Includes retry logic with exponential backoff.
        /// </summary>
        /// <param name="requestUri">The URI the request is sent to.</param>
        /// <param name="attempts">The maximum number of attempts to make before giving up (default is 5).</param>
        /// <returns>
        /// The response body as a byte array if the request was successful;
        /// otherwise, null if all attempts failed.
        /// </returns>
        /// <remarks>
        /// If a request fails, the method will wait for increasing periods between retries
        /// (100ms, 200ms, 300ms, etc.) and log the error using Serilog.
        /// </remarks>
        public async Task<byte[]?> GetByteArraySafeAsync(string requestUri, int attempts = 5)
        {
            try
            {
                foreach (var attempt in Enumerable.Range(1, attempts))
                {
                    try
                    {
                        return await _httpClient.GetByteArrayAsync(requestUri);
                    }
                    catch (Exception ex) when (attempt < attempts)
                    {
                        Log.Error(ex, "[!] Error getting byte array from {0}: {1}. Trying again in {2}ms.", requestUri, ex.Message, 100 * attempt);
                        Thread.Sleep(100 * attempt);
                    }
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error getting byte array from {0}: {1}.", requestUri, ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Sends a GET request to the specified URI and returns the response body as a string.
        /// Includes retry logic with exponential backoff.
        /// </summary>
        /// <param name="requestUri">The URI the request is sent to.</param>
        /// <param name="attempts">The maximum number of attempts to make before giving up (default is 5).</param>
        /// <returns>
        /// The response body as a string if the request was successful;
        /// otherwise, null if all attempts failed.
        /// </returns>
        /// <remarks>
        /// If a request fails, the method will wait for increasing periods between retries
        /// (100ms, 200ms, 300ms, etc.) and log the error using Serilog.
        /// </remarks>
        public async Task<string?> GetStringSafeAsync(string requestUri, int attempts = 5)
        {
            try
            {
                foreach (var attempt in Enumerable.Range(1, attempts))
                {
                    try
                    {
                        return await _httpClient.GetStringAsync(requestUri);
                    }
                    catch (Exception ex) when (attempt < attempts)
                    {
                        Log.Error(ex, "[!] Error getting string from {0}: {1}. Trying again in {2}ms.", requestUri, ex.Message, 100 * attempt);
                        Thread.Sleep(100 * attempt);
                    }
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[!] Error getting string from {0}: {1}.", requestUri, ex.Message);

                return null;
            }
        }
    }
}