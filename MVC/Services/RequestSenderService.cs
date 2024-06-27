using System.Text.Json;
using System.Text;

namespace MVC.Services
{
    public class RequestSenderService : IRequestSenderService
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
    }
}
