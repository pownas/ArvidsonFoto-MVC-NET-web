using System.Text.Json;
using ArvidsonFoto.Core.Interfaces;

namespace ArvidsonFoto.Core.Services;

/// <summary>
/// Service för att integrera med Facebook Graph API
/// </summary>
public class FacebookService : IFacebookService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private const string GraphApiBaseUrl = "https://graph.facebook.com/v19.0";

    public FacebookService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public bool IsConfigured()
    {
        var pageAccessToken = _configuration["Facebook:PageAccessToken"];
        var pageId = _configuration["Facebook:PageId"];
        
        return !string.IsNullOrWhiteSpace(pageAccessToken) && !string.IsNullOrWhiteSpace(pageId);
    }

    public async Task<string?> CreatePostAsync(List<string> imageUrls, string message)
    {
        try
        {
            if (!IsConfigured())
            {
                Log.Error("Facebook service är inte konfigurerad. Saknar PageAccessToken eller PageId.");
                return null;
            }

            if (imageUrls == null || imageUrls.Count == 0)
            {
                Log.Error("Ingen bild angiven för Facebook-inlägg.");
                return null;
            }

            var pageAccessToken = _configuration["Facebook:PageAccessToken"];
            var pageId = _configuration["Facebook:PageId"];

            // För en bild: använd photo endpoint
            // För flera bilder: använd feed endpoint med flera bilder
            if (imageUrls.Count == 1)
            {
                return await CreateSinglePhotoPostAsync(pageId!, pageAccessToken!, imageUrls[0], message);
            }
            else
            {
                return await CreateMultiPhotoPostAsync(pageId!, pageAccessToken!, imageUrls, message);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fel vid skapande av Facebook-inlägg");
            return null;
        }
    }

    private async Task<string?> CreateSinglePhotoPostAsync(string pageId, string accessToken, string imageUrl, string message)
    {
        try
        {
            var endpoint = $"{GraphApiBaseUrl}/{pageId}/photos";
            
            var parameters = new Dictionary<string, string>
            {
                { "url", imageUrl },
                { "caption", message },
                { "access_token", accessToken }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(endpoint, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);
                
                if (jsonResponse != null && jsonResponse.ContainsKey("id"))
                {
                    var postId = jsonResponse["id"].GetString();
                    // Facebook photo posts har URL-format: https://www.facebook.com/{pageId}/photos/{photoId}
                    return $"https://www.facebook.com/{pageId}/photos/{postId}";
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Log.Error($"Facebook API-fel vid skapande av foto-inlägg: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fel vid skapande av Facebook foto-inlägg");
        }

        return null;
    }

    private async Task<string?> CreateMultiPhotoPostAsync(string pageId, string accessToken, List<string> imageUrls, string message)
    {
        try
        {
            // Steg 1: Ladda upp varje bild och få media-ID:n
            var mediaIds = new List<string>();
            
            foreach (var imageUrl in imageUrls)
            {
                var mediaId = await UploadPhotoForAlbumAsync(pageId, accessToken, imageUrl);
                if (!string.IsNullOrEmpty(mediaId))
                {
                    mediaIds.Add(mediaId);
                }
            }

            if (mediaIds.Count == 0)
            {
                Log.Error("Kunde inte ladda upp några bilder till Facebook");
                return null;
            }

            // Logga varning om några bilder misslyckades
            if (mediaIds.Count < imageUrls.Count)
            {
                Log.Warning($"Endast {mediaIds.Count} av {imageUrls.Count} bilder kunde laddas upp till Facebook");
            }

            // Steg 2: Skapa inlägg med alla media-ID:n
            var endpoint = $"{GraphApiBaseUrl}/{pageId}/feed";
            
            var parameters = new Dictionary<string, string>
            {
                { "message", message },
                { "access_token", accessToken }
            };

            // Lägg till varje bild som attached_media
            for (int i = 0; i < mediaIds.Count; i++)
            {
                parameters.Add($"attached_media[{i}]", $"{{\"media_fbid\":\"{mediaIds[i]}\"}}");
            }

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(endpoint, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);
                
                if (jsonResponse != null && jsonResponse.ContainsKey("id"))
                {
                    var postId = jsonResponse["id"].GetString();
                    // Facebook feed posts har URL-format: https://www.facebook.com/{postId}
                    return $"https://www.facebook.com/{postId}";
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Log.Error($"Facebook API-fel vid skapande av multi-foto-inlägg: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fel vid skapande av Facebook multi-foto-inlägg");
        }

        return null;
    }

    private async Task<string?> UploadPhotoForAlbumAsync(string pageId, string accessToken, string imageUrl)
    {
        try
        {
            var endpoint = $"{GraphApiBaseUrl}/{pageId}/photos";
            
            var parameters = new Dictionary<string, string>
            {
                { "url", imageUrl },
                { "published", "false" }, // Ladda upp utan att publicera direkt
                { "access_token", accessToken }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(endpoint, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);
                
                if (jsonResponse != null && jsonResponse.ContainsKey("id"))
                {
                    return jsonResponse["id"].GetString();
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Log.Error($"Facebook API-fel vid uppladdning av foto: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fel vid uppladdning av foto till Facebook");
        }

        return null;
    }
}
