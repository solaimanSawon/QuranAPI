using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebBack.Services
{
    public class QuranApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        private const string TOKEN_CACHE_KEY = "QuranOAuthToken";

        public QuranApiService(HttpClient httpClient, IMemoryCache cache, IConfiguration config)
        {
            _httpClient = httpClient;
            _cache = cache;
            _config = config;

            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) WebBackApp/1.0");
            }
        }

        // ১. OAuth Token নেওয়া ও ক্যাশ করা
        public async Task<string> GetAccessTokenAsync()
        {
            if (_cache.TryGetValue(TOKEN_CACHE_KEY, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
            {
                return cachedToken;
            }

            var tokenUrl = _config["QuranApi:TokenUrl"]?.Trim();
            var clientId = _config["QuranApi:ClientId"]?.Trim();
            var clientSecret = _config["QuranApi:ClientSecret"]?.Trim();

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

            // Quran Foundation HTTP Basic Auth গ্রহণ করে (clientId:clientSecret)
            var authBytes = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            var base64Auth = Convert.ToBase64String(authBytes);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);

            // Form Content: grant_type & scope
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "content")
            });

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"OAuth Token Request Failed ({response.StatusCode}): {errorContent}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonString);

            if (tokenResponse?.AccessToken == null)
            {
                throw new Exception("OAuth Token পেতে ব্যর্থ হয়েছে।");
            }

            var expiresInSeconds = tokenResponse.ExpiresIn > 300 ? tokenResponse.ExpiresIn - 300 : tokenResponse.ExpiresIn;
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(expiresInSeconds));

            _cache.Set(TOKEN_CACHE_KEY, tokenResponse.AccessToken, cacheOptions);

            return tokenResponse.AccessToken;
        }

        // ২. Quran API Call করা
        public async Task<string> GetQuranDataAsync(string endpoint)
        {
            var token = await GetAccessTokenAsync();
            var baseUrl = _config["QuranApi:BaseUrl"]?.Trim();
            var clientId = _config["QuranApi:ClientId"]?.Trim();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}{endpoint}");

            // Quran Foundation Required Request Headers
            request.Headers.TryAddWithoutValidation("x-auth-token", token);
            request.Headers.TryAddWithoutValidation("x-client-id", clientId);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Quran API Request Failed ({response.StatusCode}): {errorContent}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}