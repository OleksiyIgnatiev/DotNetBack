using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNetBack.Services
{
    public class ImageSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ImageSearchService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> GetImageUrlAsync(string query)
        {
            try
            {
                string url = $"https://api.bing.microsoft.com/v7.0/images/search?q={query}&count=1";

                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

                string imageUrl = data?.value?[0]?.contentUrl;

                return imageUrl;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
