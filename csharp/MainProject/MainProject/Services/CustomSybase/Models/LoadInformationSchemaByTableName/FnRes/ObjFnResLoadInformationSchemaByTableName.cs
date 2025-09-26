using MainProject.Services.CustomSybase.Models.LoadInformationSchemaByTableName.Json;

namespace MainProject.Services.CustomSybase.Models.LoadInformationSchemaByTableName.FnRes
{
    public class ObjFnResLoadInformationSchemaByTableName
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<InformationSchemaJsonModel> Data { get; set; } = new List<InformationSchemaJsonModel>();
    }
}
