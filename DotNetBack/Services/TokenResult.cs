using Newtonsoft.Json;

namespace DotNetBack.Services
{
    public class TokenResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
