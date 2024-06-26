using System.Text.Json;
using System.Text;

namespace MVC.Services
{
    public class RequestSenderService
    {
        public async Task<HttpResponseMessage> GetRequest(string apiUrl)
        {
            using HttpClient httpClient = new HttpClient();
            return await httpClient.GetAsync(apiUrl);
        }

        public async Task<HttpResponseMessage> PostRequest<T>(T obj, string apiUrl)
        {
            using HttpClient httpClient = new HttpClient();
            string jsonObj = JsonSerializer.Serialize(obj);
            var content = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(apiUrl, content);
        }

        public async Task<HttpResponseMessage> PatchRequest<T>(T obj, string apiUrl)
        {
            using HttpClient httpClient = new HttpClient();
            string jsonObj = JsonSerializer.Serialize(obj);
            var content = new StringContent(jsonObj, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), apiUrl)
            {
                Content = content
            };

            return await httpClient.SendAsync(request);
        }
    }
}
