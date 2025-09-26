using MainProject.Controllers.CustomClientes.Models.JsonParsed;

namespace MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.ObtainClientesObjectListAzync.FnRes
{
    public class ObjFnResObtainClientesObjList
    {
        // Required ExternalProvider MINIMAL
        // ADD HERE some properties required by external provider like bancos



        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ClienteJsonModelFiltered> Kdata { get; set; } = new List<ClienteJsonModelFiltered>();



        // EXTRA INFO        
        public string Db_name_input { get; set; } = "";
        public string Db_name_output { get; set; } = "";
    }
}
