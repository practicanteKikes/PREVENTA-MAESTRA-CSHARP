namespace MainProject.Controllers.CustomSwagger.Services
{
    public interface ICustomSwagger
    {
        Task<string> MainCreateCookieLoginDatabaseAsync(string ls_json);
    }
}
