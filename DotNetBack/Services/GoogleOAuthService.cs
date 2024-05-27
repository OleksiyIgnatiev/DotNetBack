namespace DotNetBack.Services
{
    public class GoogleOAuthService
    {

        private const string ClientId = "845923484613-57s707glu5bu505ugqtgmkfb363aphft.apps.googleusercontent.com";
        private const string ClientSecret = "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";


        public static string GenerateOAuthRequestUrl()
        {
            throw new NotImplementedException();
        }

        public static object ExchangeCodeOnToken()
        {
            throw new NotImplementedException();
        }

        public static object RefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
