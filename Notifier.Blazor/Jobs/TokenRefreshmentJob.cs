using Microsoft.Extensions.Options;
using Notifier.Blazor.Settings;
using Notifier.Logic.Services;
using Notifier.Vk.Contract;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Diagnostics.Metrics;

namespace Notifier.Blazor.Jobs
{
    internal class TokenRefreshmentJob : Job<TokenRefreshmentJob>
    {
        private const string _loginInput = "input[name='login']";
        private const string _submitButton = "button[type='submit']";
        private const string _passwordInput = "input[type='password']";
        private const string _otpInput = "input[name='otp']";
        private const string _continueAsButton = "button[data-test-id='continue-as-button']";

        private readonly By _loginInputSelector = By.CssSelector(_loginInput);
        private readonly By _submitButtonSelector = By.CssSelector(_submitButton);
        private readonly By _passwordInputSelector = By.CssSelector(_passwordInput);
        private readonly By _otpInputSelector = By.CssSelector(_otpInput);
        private readonly By _continueAsButtonSelector = By.CssSelector(_continueAsButton);

        private readonly IVkAuthenticationRestClientBuilder _authenticationClientBuilder;
        private readonly AccessTokenService _accessTokenService;
        private readonly IOptions<VkApiSettings> _apiSettings;
        private readonly Lazy<IWebDriver> _chromeDriver;

        public TokenRefreshmentJob(
            IVkAuthenticationRestClientBuilder authenticationClientBuilder,
            AccessTokenService accessTokenService,
            IOptions<VkApiSettings> apiSettings,
            Lazy<IWebDriver> chromeDriver,
            ILogger<TokenRefreshmentJob> logger,
            IMeterFactory meterFactory)
            : base(logger, meterFactory)
        {
            _authenticationClientBuilder = authenticationClientBuilder;
            _accessTokenService = accessTokenService;
            _apiSettings = apiSettings;
            _chromeDriver = chromeDriver;
        }

        protected override async Task Run()
        {
            var (_, validThrough) = await _accessTokenService.GetAccessToken();

            if (validThrough - DateTimeOffset.Now > TimeSpan.FromMinutes(10))
            {
                return;
            }

            _logger.LogInformation("Starting to refresh Vk token");

            using var driver = _chromeDriver.Value;

            //Because we're not really "redirecting back" anywhere (the redirect url is not working, and we're only interested in "code" parameter passed back by VK api redirection),
            //this url can be anything that is valid url and added in VK application settings as trusted

            //var endAuthenticationUrl = _navigationManager.BaseUri + ApiControllerRoutesHelper.GetActionUrl<VkTokenController>(c => c.EndImplicitAuthentication);

            using var authenticationClient = _authenticationClientBuilder
                .WithClientId(_apiSettings.Value.ClientId)
                .WithClientSecret(_apiSettings.Value.ClientSecret)
                .WithScope("friends")
                .WithScope("video")
                .WithRedirectUrl(_apiSettings.Value.RedirectUrl)
                .Build();

            var authenticationQueryString = authenticationClient.GetAuthenticationQueryString();
            driver.Navigate().GoToUrl(authenticationQueryString);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            wait.Until(ExpectedConditions.ElementIsVisible(_loginInputSelector));
            wait.Until(ExpectedConditions.ElementIsVisible(_submitButtonSelector));

            driver.FindElement(_loginInputSelector).SendKeys(_apiSettings.Value.Login);
            driver.FindElement(_submitButtonSelector).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(_passwordInputSelector));
            wait.Until(ExpectedConditions.ElementIsVisible(_submitButtonSelector));

            driver.FindElement(_passwordInputSelector).SendKeys(_apiSettings.Value.Password);
            driver.FindElement(_submitButtonSelector).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(_otpInputSelector));
            wait.Until(ExpectedConditions.ElementIsVisible(_submitButtonSelector));

            var otp = new AuthenticatorService(_apiSettings.Value.AuthenticationSecret).GetAuthenticationCode(DateTimeOffset.UtcNow);

            driver.FindElement(_otpInputSelector).SendKeys(otp);
            driver.FindElement(_submitButtonSelector).Click();

            wait.Until(ExpectedConditions.ElementIsVisible(_continueAsButtonSelector));

            driver.FindElement(_continueAsButtonSelector).Click();

            wait.Until(ExpectedConditions.UrlContains("?code="));

            //Not entirely correct but works
            var code = driver.Url[(driver.Url.IndexOf("?code=") + 6)..];

            var token = await authenticationClient.GetAccessToken(code);

            await _accessTokenService.StoreAccessToken(token.Token, token.ExpiresIn);

            _logger.LogInformation("Vk token stored, valid for {Time}", TimeSpan.FromSeconds(token.ExpiresIn));
        }
    }
}
