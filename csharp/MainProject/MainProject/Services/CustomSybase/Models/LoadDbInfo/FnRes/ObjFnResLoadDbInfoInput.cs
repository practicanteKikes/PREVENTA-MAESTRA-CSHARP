using MainProject.Services.CustomSybase.Models.LoadDbInfo.Json;

namespace MainProject.Services.CustomSybase.Models.LoadDbInfo.FnRes
{
    public class ObjFnResLoadDbInfoInput
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<DatabaseInfoInputJsonModel> Data { get; set; } = new List<DatabaseInfoInputJsonModel>();
    }
}
