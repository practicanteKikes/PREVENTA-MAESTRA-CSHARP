using MainProject.Controllers.CurrentProject.Models.QueryUsersFilteredAzync.JsonParsed;
using MainProject.Services.CustomDatabaseInput.Models.Json;

namespace MainProject.Controllers.CurrentProject.Models.FnRes
{
    public class ObjFnResBdConsultarUsersJsonParsed
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<UserInputJsonParsed> Data { get; set; } = new List<UserInputJsonParsed>(); // Por defecto un array vacio
    }
}
