using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Web;

namespace SearchEngine
{

    public class SerpApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BASE_URL = "https://serpapi.com/search";

        public SerpApiService(string apiKey)
        {
            _httpClient = _httpClient ?? new HttpClient();
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<List<SearchData>> SearchAsync(string query)
        {
            try
            {
                // Construct query parameters
                var queryParams = HttpUtility.ParseQueryString(string.Empty);
                queryParams.Add("engine", "google");
                queryParams.Add("q", query);
                queryParams.Add("api_key", _apiKey);
                queryParams.Add("num", "10"); // Number of results
                queryParams.Add("hl", "en"); // Language
                queryParams.Add("gl", "us"); // Country

                // Build full URL
                var uriBuilder = new UriBuilder(BASE_URL);
                uriBuilder.Query = queryParams.ToString();

                // Send HTTP request
                var response = await _httpClient.GetAsync(uriBuilder.ToString());
                response.EnsureSuccessStatusCode();

                // Read response content
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse and return results
                return SearchData.ParseFromJson(responseBody);
            }
            catch (HttpRequestException ex)
            {
                // Log the exception
                Console.WriteLine($"Search API Error: {ex.Message}");
                return new List<SearchData>();
            }
        }
    }
}