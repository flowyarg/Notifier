namespace Notifier.Vk.Implementation;

using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Models.VkVideo;

public partial class VkVideoApiCredentialsService
{
    private static readonly Regex WebChunksParseRegex = CreateWebChunksParsingRegex();
    private static readonly Regex ApiVersionParsingRegex = CreateApiVersionParsingRegex();
    private static readonly Regex ClientSecretParsingRegex = CreateClientSecretParsingRegex();
    private static readonly Regex ClientIdParsingRegex = CreateClientIdParsingRegex();
    private static readonly Regex AppIdParsingRegex = CreateAppIdParsingRegex();

    private readonly VkVideoAuthRestClient _authRestClient;
    private readonly ILogger<VkVideoApiCredentialsService> _logger;
    
    private VkVideoApiCredentials? _credentials;

    public VkVideoApiCredentialsService(VkVideoAuthRestClient authRestClient, ILogger<VkVideoApiCredentialsService> logger)
    {
        _authRestClient = authRestClient;
        _logger = logger;
    }

    public async Task<VkVideoApiCredentials> GetCredentials()
    {
        _credentials ??= await GenerateCredentials();
        return _credentials;
    }

    public async Task<VkVideoApiCredentials> GetNewCredentials()
    {
        _credentials = await GenerateCredentials();
        return _credentials;
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
            AccessToken = accessToken
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