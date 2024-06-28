using System.Text.Json;
using System.Text;

namespace MVC.Services
{
    public class RequestSenderService : IRequestSenderService
    {
        private readonly HttpClient _httpClient;
        public RequestSenderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<HttpResponseMessage> GetRequest(string apiUrl)
        {
            return await _httpClient.GetAsync(apiUrl);
        }

        public async Task<HttpResponseMessage> PostRequest<T>(T obj, string apiUrl)
        {
            string jsonObj = JsonSerializer.Serialize(obj);
            var content = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(apiUrl, content);
        }

    }
}
