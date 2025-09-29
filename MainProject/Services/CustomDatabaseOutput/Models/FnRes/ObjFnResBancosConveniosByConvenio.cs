using MainProject.Services.CustomDatabaseOutput.Models.Json;

namespace MainProject.Services.CustomDatabaseOutput.Models.FnRes
{
    public class ObjFnResBancosConveniosByConvenio
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<BancoConvenioOutputJsonModel> Data { get; set; } = new List<BancoConvenioOutputJsonModel>();
    }
}
