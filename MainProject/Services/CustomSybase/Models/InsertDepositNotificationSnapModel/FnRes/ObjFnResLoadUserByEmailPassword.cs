using MainProject.Services.CustomDatabaseInput.Models.Json;

namespace MainProject.Services.CustomSybase.Models.InsertDepositNotificationSnapModel.FnRes
{
    public class ObjFnResLoadUserByEmailPassword
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<ClienteJsonModel> Data { get; set; } = new List<ClienteJsonModel>();
    }
}
