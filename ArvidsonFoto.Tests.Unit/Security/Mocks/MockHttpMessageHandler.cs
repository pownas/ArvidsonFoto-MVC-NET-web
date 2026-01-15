namespace ArvidsonFoto.Tests.Unit.Security.Mocks;

/// <summary>
/// Mock implementation of HttpMessageHandler for testing purposes
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private HttpResponseMessage? _response;
    private Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? _sendAsyncFunc;

    public List<HttpRequestMessage> Requests { get; } = new();

    public void SetResponse(HttpResponseMessage response)
    {
        _response = response;
    }

    public void SetResponseFunc(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> func)
    {
        _sendAsyncFunc = func;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);

        if (_sendAsyncFunc != null)
        {
            return _sendAsyncFunc(request, cancellationToken);
        }

        if (_response != null)
        {
            return Task.FromResult(_response);
        }

        // Default response if nothing is configured
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("")
        });
    }
}
