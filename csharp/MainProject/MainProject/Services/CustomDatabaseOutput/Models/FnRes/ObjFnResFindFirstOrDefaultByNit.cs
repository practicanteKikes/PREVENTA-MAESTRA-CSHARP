using MainProject.Services.CustomDatabaseOutput.Models.Json;

namespace MainProject.Services.CustomDatabaseOutput.Models.FnRes
{
    public class ObjFnResFindFirstOrDefaultByNit
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<VwOccidenteTercerosOutputJsonModel> Data { get; set; } = new List<VwOccidenteTercerosOutputJsonModel>();
    }
}
