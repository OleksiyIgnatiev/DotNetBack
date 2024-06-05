using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DotNetBack.Services
{
    public class GoogleApiService
    {
        private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

        public static async Task<string> GetUserEmailAsync(string accessToken)
        {
            // Формируем запрос к Google API для получения информации о пользователе
            var requestUrl = $"{UserInfoEndpoint}?access_token={accessToken}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(requestUrl);

            // Проверяем, успешно ли выполнен запрос
            response.EnsureSuccessStatusCode();

            // Читаем содержимое ответа
            var responseContent = await response.Content.ReadAsStringAsync();

            // Десериализуем JSON-ответ
            var userInfo = JsonConvert.DeserializeObject<UserInfoResponse>(responseContent);

            // Возвращаем адрес электронной почты пользователя
            return userInfo.Email;
        }

        private class UserInfoResponse
        {
            public string Email { get; set; }
        }
    }
}
