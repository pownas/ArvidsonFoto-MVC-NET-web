namespace ArvidsonFoto.Services;

public interface IFacebookService
{
    //Task<Account> GetAccountAsync(string accessToken);
    Task PostOnWallAsync(string accessToken, string message);
}

/// <summary>
/// Service to connect to Facebook API, and send API-calls
/// Code are partly from this blogpost: https://piotrgankiewicz.com/2017/02/06/accessing-facebook-api-using-c/
/// </summary>
public class FacebookApiService : IFacebookService
{
    private readonly IFacebookClient _facebookClient;

    public FacebookApiService(IFacebookClient facebookClient)
    {
        _facebookClient = facebookClient;
    }

    //public async Task<Account> GetAccountAsync(string accessToken)
    //{
    //    var result = await _facebookClient.GetAsync<dynamic>(
    //        accessToken, "me", "fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale");

    //    if (result == null)
    //    {
    //        return new Account();
    //    }

    //    var account = new Account
    //    {
    //        Id = result.id,
    //        Email = result.email,
    //        Name = result.name,
    //        UserName = result.username,
    //        FirstName = result.first_name,
    //        LastName = result.last_name,
    //        Locale = result.locale
    //    };

    //    return account;
    //}

    public async Task PostOnWallAsync(string accessToken, string message)
        => await _facebookClient.PostAsync(accessToken, "me/feed", new { message });
}