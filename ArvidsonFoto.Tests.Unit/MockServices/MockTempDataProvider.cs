using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of ITempDataProvider for unit testing.
/// </summary>
public class MockTempDataProvider : ITempDataProvider
{
    private readonly Dictionary<string, object?> _data = new();

    public IDictionary<string, object?> LoadTempData(HttpContext context)
    {
        return _data;
    }

    public void SaveTempData(HttpContext context, IDictionary<string, object?> values)
    {
        _data.Clear();
        foreach (var kvp in values)
        {
            _data[kvp.Key] = kvp.Value;
        }
    }
}
