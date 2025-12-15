using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ArvidsonFoto.Tests.Unit.Security.Mocks;

/// <summary>
/// Mock implementation of IConfiguration for testing purposes
/// </summary>
public class MockConfiguration : IConfiguration
{
    private readonly Dictionary<string, string?> _values = new();

    public string? this[string key]
    {
        get => _values.TryGetValue(key, out var value) ? value : null;
        set => _values[key] = value;
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return Enumerable.Empty<IConfigurationSection>();
    }

    public IChangeToken GetReloadToken()
    {
        return new ConfigurationReloadToken();
    }

    public IConfigurationSection GetSection(string key)
    {
        return new MockConfigurationSection(key, this);
    }

    public void SetValue(string key, string? value)
    {
        _values[key] = value;
    }

    private class ConfigurationReloadToken : IChangeToken
    {
        public bool HasChanged => false;
        public bool ActiveChangeCallbacks => false;
        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state) => new EmptyDisposable();
    }

    private class EmptyDisposable : IDisposable
    {
        public void Dispose() { }
    }

    private class MockConfigurationSection : IConfigurationSection
    {
        private readonly string _key;
        private readonly MockConfiguration _configuration;

        public MockConfigurationSection(string key, MockConfiguration configuration)
        {
            _key = key;
            _configuration = configuration;
        }

        public string? this[string key]
        {
            get => _configuration[$"{_key}:{key}"];
            set => _configuration[$"{_key}:{key}"] = value;
        }

        public string Key => _key;
        public string Path => _key;
        public string? Value
        {
            get => _configuration[_key];
            set => _configuration[_key] = value;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return Enumerable.Empty<IConfigurationSection>();
        }

        public IChangeToken GetReloadToken()
        {
            return new ConfigurationReloadToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            return new MockConfigurationSection($"{_key}:{key}", _configuration);
        }
    }
}
