using Microsoft.Extensions.Caching.Memory;

namespace MainProject.Services.CustomTokenCache.Impl
{
    public class CustomTokenCacheService : ICustomTokenCache
    {
        private readonly IMemoryCache _memoryCache;

        public CustomTokenCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGetToken(string key, out string ls_token)
        {            
            string? ls_token_or_null = null; // declar variable to be used inside TryGetValue
            bool lb_token_obtained = _memoryCache.TryGetValue(key, out ls_token_or_null); // return boolean and set the out
            
            ls_token = ls_token_or_null ?? ""; // Asignamos token antes de salir
            return lb_token_obtained;
        }

        public void AddToken(string key, string token, TimeSpan expiration)
        {
            _memoryCache.Set(key, token, expiration);
        }
    }
}
