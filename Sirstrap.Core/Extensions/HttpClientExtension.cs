using Serilog;

namespace Sirstrap.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for HttpClient with built-in retry capabilities.
    /// </summary>
    /// <remarks>
    /// This class wraps HttpClient methods with automatic retry logic using exponential backoff.
    /// It logs failures and retry attempts using Serilog.
    /// </remarks>
    /// <param name="httpClient">The HttpClient instance to extend.</param>
    public class HttpClientExtension //v1.1.5.2-hcedev => v1.1.5.3-beta
    {
        private const int DEFAULT_ATTEMPTS = 5;

        private readonly HttpClient _httpClient;

        public HttpClientExtension()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
        }

        /// <summary>
        /// Retrieves the contents of the specified URI as a byte array with retry capability.
        /// </summary>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="attempts">The maximum number of retry attempts. Defaults to 5.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the 
        /// contents of the response body as a byte array, or an empty byte array if all attempts fail.
        /// </returns>
        /// <remarks>
        /// This method will automatically retry failed requests with exponential backoff.
        /// </remarks>
        public async Task<byte[]> GetByteArrayAsync(string requestUri, int attempts = DEFAULT_ATTEMPTS)
        {
            return await ExecuteWithRetryAsync(async () => await _httpClient.GetByteArrayAsync(requestUri).ConfigureAwait(false), requestUri, "BYTEARRAY", attempts).ConfigureAwait(false) ?? [];
        }

        /// <summary>
        /// Retrieves the contents of the specified URI as a string with retry capability.
        /// </summary>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="attempts">The maximum number of retry attempts. Defaults to 5.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the 
        /// contents of the response body as a string, or an empty string if all attempts fail.
        /// </returns>
        /// <remarks>
        /// This method will automatically retry failed requests with exponential backoff.
        /// </remarks>
        public async Task<string> GetStringAsync(string requestUri, int attempts = DEFAULT_ATTEMPTS)
        {
            return await ExecuteWithRetryAsync(async () => await _httpClient.GetStringAsync(requestUri).ConfigureAwait(false), requestUri, "STRING", attempts).ConfigureAwait(false) ?? string.Empty;
        }

        #region PRIVATE METHODS

        /// <summary>
        /// Executes the specified operation with retry capability and exponential backoff.
        /// </summary>
        /// <typeparam name="T">The return type of the operation.</typeparam>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="requestUri">The URI being requested, used for logging.</param>
        /// <param name="resourceType">The type of resource being requested, used for logging.</param>
        /// <param name="attempts">The maximum number of retry attempts.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the 
        /// result of the operation, or the default value for type T if all attempts fail.
        /// </returns>
        /// <remarks>
        /// On failure, this method implements exponential backoff by waiting 2^attempt seconds 
        /// before retrying. All failures are logged - warnings for retry attempts and errors 
        /// for the final failure.
        /// </remarks>
        private static async Task<T?> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, string requestUri, string resourceType, int attempts)
        {
            for (int attempt = 1; attempt <= attempts; attempt++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    if (attempt < attempts)
                    {
                        var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));

                        Log.Warning("[?][{4}] Error getting {0} from {1} (Attempt {2}/{3}). Retrying.", resourceType, requestUri, attempt, attempts, nameof(HttpClientExtension));

                        await Task.Delay(delay);
                    }
                    else
                    {
                        Log.Error(ex, "[!][{5}] Error getting {0} from {1} (Attempt {2}/{3}). Exception: {4}.", resourceType, requestUri, attempt, attempts, ex.Message, nameof(HttpClientExtension));

                        return default;
                    }
                }
            }

            return default;
        }

        #endregion PRIVATE METHODS
    }
}