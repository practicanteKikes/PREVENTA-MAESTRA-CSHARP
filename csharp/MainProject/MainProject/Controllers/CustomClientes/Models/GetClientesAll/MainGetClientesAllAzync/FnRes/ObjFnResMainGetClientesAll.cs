using MainProject.Controllers.CustomClientes.Models.GetClientesAll.MainGetClientesAllAzync.Json;
using MainProject.Services.CustomDatabaseInput.Models.Json;

namespace MainProject.Controllers.CustomClientes.Models.GetClientesAll.MainGetClientesAllAzync.FnRes
{
    public class ObjFnResMainGetClientesAll
    {
        // Required ExternalProvider MINIMAL
        // ADD HERE some properties required by external provider like bancos



        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ClienteJsonCustomized> Kdata { get; set; } = new List<ClienteJsonCustomized>();
    }
}
