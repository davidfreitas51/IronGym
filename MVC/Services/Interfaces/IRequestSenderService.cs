namespace MVC.Services
{
    public interface IRequestSenderService
    {
        Task<HttpResponseMessage> GetRequest(string apiUrl);
        Task<HttpResponseMessage> PostRequest<T>(T obj, string apiUrl);
    }
}
