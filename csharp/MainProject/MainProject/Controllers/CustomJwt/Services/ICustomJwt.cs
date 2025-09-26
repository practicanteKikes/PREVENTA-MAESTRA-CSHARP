using MainProject.Controllers.CustomJwt.Models.DbString;
using System.Security.Claims;

namespace MainProject.Controllers.CustomJwt.Services
{
    public interface ICustomJwt
    {
        string TokenStatus();

        string TokenCreate(string ls_json);

        string EntityUsers();

        string TokenStatusDatabase();

        string TokenCreateDatabase(string ls_json);
        string ReadUserPasswordFromHeaders();

        bool tokenHttpUserIsAuthenticated();

        (bool status, string message, List<ClaimsIdentity> data) getMergedIdentityWithClaims();

        (bool status, string message, List<UserJwtDbStringModel> data) getUserJwtByUserAndPassword(Dictionary<string, object> la_params);
    }
}
