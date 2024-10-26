using System.Web;
using SpotifyDeviceFlow.Helpers;
using SpotifyDeviceFlow.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder.Build();

string clientId = builder.Configuration["Spotify:ClientId"] ?? "";
string clientSecret = builder.Configuration["Spotify:ClientSecret"] ?? "";
string key = builder.Configuration["EncryptionKey"] ?? "";

if (
    string.IsNullOrEmpty(clientId)
    || string.IsNullOrEmpty(clientSecret)
    || string.IsNullOrEmpty(key)
)
{
    throw new InvalidOperationException(
        "ClientId, ClientSecret, and EncryptionKey must be provided. Check README.md for more information."
    );
}

List<Code> codes = [new Code { CodeValue = "123" }];

Task task = Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(TimeSpan.FromSeconds(30));
        DateTime now = DateTime.UtcNow;
        _ = codes.RemoveAll(c => c.ExpiryTime < now);
    }
});

app.MapPost(
    "/api/code",
    (CodeRequest codeRequest) =>
    {
        if (codes.Exists(c => c.CodeValue == codeRequest.Code))
        {
            return Results.BadRequest(new { error = "Code already exists" });
        }

        Code code = new() { CodeValue = codeRequest.Code };
        codes.Add(code);
        return Results.Created($"/api/poll/{code.CodeValue}", code);
    }
);

app.MapGet(
    "/api/poll/{code}",
    (string code) =>
    {
        Code? codeObj = codes.Find(c => c.CodeValue == code);
        if (codeObj == null)
        {
            return Results.NotFound();
        }

        if (codeObj.Token == string.Empty)
        {
            return Results.NoContent();
        }

        _ = codes.Remove(codeObj);
        return Results.Ok(codeObj.Token);
    }
);

app.MapGet(
    "/api/login",
    (string code) =>
    {
        if (code == null)
        {
            return Results.BadRequest(new { error = "Invalid code" });
        }

        string hashedState = Crypto.Encrypt(code, key);
        string scopes = "streaming user-read-email user-read-private";
        return Results.Redirect(
            $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri=http://fedora:5000/api/callback&scope={scopes}&state={hashedState}"
        );
    }
);

app.MapGet(
    "/api/callback",
    async (string code, string state) =>
    {
        Code? existingCode = codes.Find(c =>
            Crypto.Descrypt(HttpUtility.UrlDecode(state), key) == c.CodeValue
        );
        if (existingCode == null)
        {
            return Results.BadRequest(new { error = "Invalid code" });
        }

        using HttpClient client = new();
        HttpRequestMessage tokenRequest =
            new(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        List<KeyValuePair<string, string>> parameters =
        [
            new("grant_type", "authorization_code"),
            new("code", code),
            new("redirect_uri", "http://fedora:5000/api/callback"),
            new("client_id", clientId),
            new("client_secret", clientSecret),
        ];
        tokenRequest.Content = new FormUrlEncodedContent(parameters);

        HttpResponseMessage response = await client.SendAsync(tokenRequest);
        if (!response.IsSuccessStatusCode)
        {
            return Results.BadRequest(new { error = "Failed to obtain access token" });
        }

        string responseContent = await response.Content.ReadAsStringAsync();
        existingCode.Token = responseContent;
        return Results.Ok(responseContent);
    }
);

app.MapPost(
    "/api/refresh",
    async (RefreshTokenRequest refreshTokenRequest) =>
    {
        HttpClient client = new();
        HttpRequestMessage refreshRequest =
            new(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        List<KeyValuePair<string, string>> parameters =
        [
            new("grant_type", "refresh_token"),
            new("refresh_token", refreshTokenRequest.RefreshToken),
            new("client_id", clientId),
            new("client_secret", clientSecret),
        ];
        refreshRequest.Content = new FormUrlEncodedContent(parameters);

        HttpResponseMessage response = await client.SendAsync(refreshRequest);
        if (!response.IsSuccessStatusCode)
        {
            return Results.BadRequest(new { error = "Failed to refresh access token" });
        }

        string responseContent = await response.Content.ReadAsStringAsync();
        return Results.Ok(responseContent);
    }
);

app.Run();
