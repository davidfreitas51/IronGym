using FakeItEasy;
using FluentAssertions;
using MVC.Services;
using System.Net;
using System.Text;
using System.Text.Json;

public class RequestSenderServiceTests
{
    private readonly IRequestSenderService _requestSenderService;

    public RequestSenderServiceTests()
    {
        _requestSenderService = A.Fake<IRequestSenderService>();
    }

    private async Task<HttpResponseMessage> SetupAndExecuteGetRequest(string apiUrl, HttpResponseMessage expectedResponse)
    {
        A.CallTo(() => _requestSenderService.GetRequest(apiUrl))
            .Returns(Task.FromResult(expectedResponse));

        return await _requestSenderService.GetRequest(apiUrl);
    }

    private async Task<HttpResponseMessage> SetupAndExecutePostRequest<T>(T obj, string apiUrl, HttpResponseMessage expectedResponse)
    {
        string jsonObj = JsonSerializer.Serialize(obj);
        var content = new StringContent(jsonObj, Encoding.UTF8, "application/json");

        A.CallTo(() => _requestSenderService.PostRequest(A<T>._, apiUrl))
            .WithAnyArguments()
            .Returns(Task.FromResult(expectedResponse));

        return await _requestSenderService.PostRequest(obj, apiUrl);
    }

    [Fact]
    public async Task GetRequest_Should_Return_Response()
    {
        string apiUrl = "https://api.example.com/data";
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        var response = await SetupAndExecuteGetRequest(apiUrl, expectedResponse);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostRequest_Should_Return_Response()
    {
        string apiUrl = "https://api.example.com/data";
        var obj = new { Id = 1, Name = "Test" };
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        var response = await SetupAndExecutePostRequest(obj, apiUrl, expectedResponse);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
