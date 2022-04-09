using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ArvidsonFoto.Data;

/// <summary>
/// Facebook Client connector to FB API. 
/// Blogpost that explains more: https://piotrgankiewicz.com/2017/02/06/accessing-facebook-api-using-c/
/// </summary>
public interface IFacebookClient
{
    Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null);
    Task PostAsync(string accessToken, string endpoint, object data, string args = null);
}

public class FacebookClient : IFacebookClient
{
    private readonly HttpClient _httpClient;

    public FacebookClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://graph.facebook.com/")
        };
        _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null)
    {
        var response = await _httpClient.GetAsync($"{endpoint}?access_token={accessToken}&{args}");
        if (!response.IsSuccessStatusCode)
            return default(T);

        var result = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(result);
    }

    public async Task PostAsync(string accessToken, string endpoint, object data, string args = null)
    {
        var payload = GetPayload(data);
        await _httpClient.PostAsync($"{endpoint}?access_token={accessToken}&{args}", payload);
    }

    private static StringContent GetPayload(object data)
    {
        var json = JsonSerializer.Serialize(data);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}