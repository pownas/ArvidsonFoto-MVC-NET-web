using Microsoft.AspNetCore.Http;

namespace ArvidsonFoto.Tests.Unit.Security.Mocks;

/// <summary>
/// Mock implementation of RequestDelegate for testing purposes
/// </summary>
public class MockRequestDelegate
{
    public int CallCount { get; private set; }
    public HttpContext? LastContext { get; private set; }

    public Task InvokeAsync(HttpContext context)
    {
        CallCount++;
        LastContext = context;
        return Task.CompletedTask;
    }

    public RequestDelegate GetDelegate()
    {
        return InvokeAsync;
    }

    public void Reset()
    {
        CallCount = 0;
        LastContext = null;
    }
}
