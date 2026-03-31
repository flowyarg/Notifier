using Microsoft.EntityFrameworkCore;
using Notifier.DataAccess;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Notifier.DataAccess.Model;
using Notifier.Vk.Models.VkVideo;

namespace Notifier.Vk.Implementation;

public partial class VkVideoApiCredentialsService
{
    private const int AccessTokenId = 1;
    
    private static readonly Regex WebChunksParseRegex = CreateWebChunksParsingRegex();
    private static readonly Regex ApiVersionParsingRegex = CreateApiVersionParsingRegex();
    private static readonly Regex ClientSecretParsingRegex = CreateClientSecretParsingRegex();
    private static readonly Regex ClientIdParsingRegex = CreateClientIdParsingRegex();
    private static readonly Regex AppIdParsingRegex = CreateAppIdParsingRegex();

    private readonly VkVideoAuthRestClient _authRestClient;
    private readonly IDbContextFactory<NotifierDbContext> _dbContextFactory;
    private readonly ILogger<VkVideoApiCredentialsService> _logger;
    
    private VkVideoApiCredentials? _credentials;

    public VkVideoApiCredentialsService(VkVideoAuthRestClient authRestClient, ILogger<VkVideoApiCredentialsService> logger, IDbContextFactory<NotifierDbContext> dbContextFactory)
    {
        _authRestClient = authRestClient;
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<VkVideoApiCredentials> GetCredentials()
    {
        _credentials ??= await LoadCredentials();
        _credentials ??= await GenerateAndStoreCredentials();
        return _credentials;
    }

    public async Task<VkVideoApiCredentials> GetNewCredentials()
    {
        _credentials = await GenerateAndStoreCredentials();
        return _credentials;
    }

    private async Task<VkVideoApiCredentials> GenerateAndStoreCredentials()
    {
        var credentials = await GenerateCredentials();
        await StoreCredentials(credentials);
        return credentials;
    }

    private async Task<VkVideoApiCredentials> GenerateCredentials()
    {
        _logger.LogInformation("Generating credentials...");
        const string scriptUrl = "https://vkvideo.ru/dist/web/chunks/core_spa.9b72bb5a.js"; //TODO: i guess it will break eventually
        Dictionary<string, string> scriptChunks;
        using (var client = new HttpClient()) // WebClient class inherits IDisposable
        {
            var scriptContent = await client.GetStringAsync(scriptUrl);
            scriptChunks = ParseWebChunks(scriptContent);
        }
        _logger.LogInformation("Script downloaded successfully");

        var apiVersion = ForceMatch(ApiVersionParsingRegex, scriptChunks["365006"]) ?? throw new FormatException("Can not parse API version");
        var clientSecret = ForceMatch(ClientSecretParsingRegex, scriptChunks["352319"]) ?? throw new FormatException("Can not parse client secret");
        var clientId = ForceMatch(ClientIdParsingRegex, scriptChunks["352319"]) ?? throw new FormatException("Can not parse client id");
        const string scopes = "audio_anonymous,video_anonymous,photos_anonymous,profile_anonymous";
        var appid = ForceMatch(AppIdParsingRegex, scriptChunks["352319"]) ?? throw new FormatException("Can not parse app id");

        var accessToken = await _authRestClient.GetAccessToken(clientSecret, clientId, scopes, appid);
        
        var credentials = new VkVideoApiCredentials
        {
            ApiVersion = apiVersion,
            ClientId = clientId,
            AccessToken = accessToken,
            Scopes = scopes
        };
        _logger.LogInformation("Credentials generated successfully");

        return credentials;
    }

    private static string? ForceMatch(Regex regex, string input)
    {
        var value = regex.Match(input).Groups[1].Value;
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
    
    private static Dictionary<string, string> ParseWebChunks(string content)
    {
        var allMatches = WebChunksParseRegex.Matches(content)
            .Where(m => m.Success)
            .Select(g => (Key: g.Groups[1].Value, StartIndex: g.Index, Content: string.Empty))
            .ToArray();

        for (var i = 0; i < allMatches.Length - 1; i++)
        {
            var currentMatch = allMatches[i];
            var nextMatch = allMatches[i + 1];
            var startIndex = currentMatch.StartIndex + currentMatch.Key.Length;
            var endIndex = nextMatch.StartIndex;

            allMatches[i] = currentMatch with { Content = content.Substring(startIndex, endIndex - startIndex) };
        }

        var lastMatch = allMatches[^1];
        allMatches[^1] = allMatches[^1] with { Content = content.Substring(lastMatch.StartIndex + lastMatch.Key.Length) };

        return allMatches
            .GroupBy(m => m.Key)
            .ToDictionary(g => g.Key, g => string.Join("_____IDK_WHAT_TO_DO_HERE_____", g.Select(gg => gg.Content)));
    }

    private async Task StoreCredentials(VkVideoApiCredentials credentials)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var accessToken = await dbContext.VkVideoAccessTokens.FindAsync(AccessTokenId);
        if (accessToken is null)
        {
            _ = dbContext.VkVideoAccessTokens.Add(new VkVideoAccessToken
            {
                Id = AccessTokenId,
                AccessToken = credentials.AccessToken,
                ClientId = credentials.ClientId,
                ApiVersion = credentials.ApiVersion,
                Scopes = credentials.Scopes
            });
        }
        else
        {
            accessToken.AccessToken = credentials.AccessToken;
            accessToken.ClientId = credentials.ClientId;
            accessToken.ApiVersion = credentials.ApiVersion;
            accessToken.Scopes = credentials.Scopes;
        }

        await dbContext.SaveChangesAsync();
    }
    
    private async Task<VkVideoApiCredentials?> LoadCredentials()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var accessToken = await dbContext.VkVideoAccessTokens.FindAsync(AccessTokenId);
        return accessToken is null
            ? null
            : new VkVideoApiCredentials
            {
                AccessToken = accessToken.AccessToken,
                ClientId = accessToken.ClientId,
                ApiVersion = accessToken.ApiVersion,
                Scopes =  accessToken.Scopes
            };
    }

    [GeneratedRegex(@"(\d+):")]
    private static partial Regex CreateWebChunksParsingRegex();

    [GeneratedRegex("""
                    const r="(.+?)"
                    """)]
    private static partial Regex CreateApiVersionParsingRegex();

    [GeneratedRegex("""
                    d="(.+?)"
                    """)]
    private static partial Regex CreateClientSecretParsingRegex();

    [GeneratedRegex(@"p=(.+?),.+")]
    private static partial Regex CreateClientIdParsingRegex();

    [GeneratedRegex(@"_=(.+?),.+")]
    private static partial Regex CreateAppIdParsingRegex();
}