using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Notifier.Blazor.Settings;
using Notifier.Logic.Services;
using Notifier.Vk.Contract;

namespace Notifier.Blazor.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    //public class VkTokenController : Controller
    //{
    //    private readonly AccessTokenService _accessTokenService;
    //    private readonly IVkAuthenticationRestClientBuilder _authenticationClientBuilder;
    //    private readonly IOptions<VkApiSettings> _apiSettings;

    //    public VkTokenController(AccessTokenService accessTokenService, IVkAuthenticationRestClientBuilder authenticationClientBuilder, IOptions<VkApiSettings> apiSettings)
    //    {
    //        _accessTokenService = accessTokenService;
    //        _authenticationClientBuilder = authenticationClientBuilder;
    //        _apiSettings = apiSettings;
    //    }

    //    [HttpGet]
    //    [Route("startAuthentication")]
    //    public IActionResult StartImplicitAuthentication()
    //    {
    //        using var authenticationClient = _authenticationClientBuilder
    //            .WithClientId(_apiSettings.Value.ClientId)
    //            .WithClientSecret(_apiSettings.Value.ClientSecret)
    //            .WithScope("friends")
    //            .WithScope("video")
    //            .WithRedirectUrl(Url.ActionLink(nameof(EndImplicitAuthentication))!)
    //            .Build();

    //        return Redirect(authenticationClient.GetAuthenticationQueryString());
    //    }

    //    [HttpGet]
    //    [Route("endAuthentication")]
    //    public async Task<IActionResult> EndImplicitAuthentication(string code, string? error = null, string? error_description = null)
    //    {
    //        using var authenticationClient = _authenticationClientBuilder
    //            .WithClientId(_apiSettings.Value.ClientId)
    //            .WithClientSecret(_apiSettings.Value.ClientSecret)
    //            .WithRedirectUrl(Url.ActionLink(nameof(EndImplicitAuthentication))!)
    //            .Build();

    //        var token = await authenticationClient.GetAccessToken(code);

    //        await _accessTokenService.StoreAccessToken(token.Token, token.ExpiresIn);

    //        return Ok("Authenticated");
    //    }
    //}
}
