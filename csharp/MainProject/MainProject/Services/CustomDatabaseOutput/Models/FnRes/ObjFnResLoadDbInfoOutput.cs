using MainProject.Services.CustomDatabaseOutput.Models.Json;

namespace MainProject.Services.CustomDatabaseOutput.Models.FnRes
{
    public class ObjFnResLoadDbInfoOutput
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<DatabaseInfoOutputJsonModel> Data { get; set; } = new List<DatabaseInfoOutputJsonModel>();
    }
}
