using MainProject.Services.CustomDatabaseInput.Models.Json;

namespace MainProject.Controllers.CustomClientes.Models.GetClientesAll.MainGetClientesAllAzync.FnRes
{
    public class ObjFnResMainGetClientesAllDev : ObjFnResMainGetClientesAll
    {
        // EXTRA INFO        
        public string Db_name_input { get; set; } = "";
        public string Db_name_output { get; set; } = "";
    }
}
