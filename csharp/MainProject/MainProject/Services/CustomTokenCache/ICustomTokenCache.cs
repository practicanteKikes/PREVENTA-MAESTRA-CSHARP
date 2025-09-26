namespace MainProject.Services.CustomTokenCache
{
    public interface ICustomTokenCache
    {
        bool TryGetToken(string key, out string token);

        void AddToken(string key, string token, TimeSpan expiration);
    }
}
