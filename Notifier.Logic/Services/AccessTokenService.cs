using Microsoft.EntityFrameworkCore;
using Notifier.DataAccess;
using Notifier.DataAccess.Model;
using Notifier.Logic.Services.Security;

namespace Notifier.Logic.Services
{
    public class AccessTokenService
    {
        private const int _accessTokenId = 1;
        private static readonly Random _random = new();

        private readonly IDbContextFactory<NotifierDbContext> _factory;
        private readonly AESCrypto _crypto;
        private readonly string _encryptionKey;

        public AccessTokenService(IDbContextFactory<NotifierDbContext> factory, AESCrypto crypto, string encryptionKey)
        {
            _factory = factory;
            _crypto = crypto;
            _encryptionKey = encryptionKey;
        }

        public async Task<(string Token, DateTimeOffset ValidThrough)> GetAccessToken()
        {
            using var context = await _factory.CreateDbContextAsync();
            var token = await context.AccessTokens.SingleOrDefaultAsync(token => token.Id == _accessTokenId);

            if (token == null)
            {
                return (string.Empty, DateTimeOffset.MinValue);
            }

            var realToken = _crypto.Decrypt(token.Token, _encryptionKey, token.IV);

            return (realToken, token.ValidThrough);
        }

        public async Task StoreAccessToken(string token, int expirationSeconds)
        {
            var validThrough = DateTimeOffset.Now.AddSeconds(expirationSeconds);

            var iv = GetRandomString(20);

            var encryptedToken = _crypto.Encrypt(token, _encryptionKey, iv);

            using var context = await _factory.CreateDbContextAsync();
            var tokenEntity = await context.AccessTokens.SingleOrDefaultAsync(token => token.Id == _accessTokenId);

            if (tokenEntity == null)
            {
                AddAccessToken(context, encryptedToken, iv, validThrough);
            }
            else
            {
                UpdateAccessToken(context, tokenEntity, encryptedToken, iv, validThrough);
            }

            await context.SaveChangesAsync();
        }

        private static void AddAccessToken(NotifierDbContext context, string token, string iv, DateTimeOffset validThrough)
        {
            context.AccessTokens.Add(new AccessToken
            {
                Id = _accessTokenId,
                Token = token,
                IV = iv,
                ValidThrough = validThrough
            });
        }

        private static void UpdateAccessToken(NotifierDbContext context, AccessToken tokenEntity, string token, string iv, DateTimeOffset validThrough)
        {
            tokenEntity.ValidThrough = validThrough;
            tokenEntity.IV = iv;
            tokenEntity.Token = token;
            context.AccessTokens.Update(tokenEntity);
        }

        private static string GetRandomString(int length)
        {
            return new string(Enumerable.Range(0, length).Select(n => (char)_random.Next(32, 127)).ToArray());
        }
    }
}
