using MainProject.Controllers.CustomClientes.Models.JsonParsed;

namespace MainProject.Controllers.CustomClientes.Models.GetClientesAll.ObtainClientesAllObjectListAzync.FnRes
{
    public class ObjFnResObtainClientesAllObjList
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ClienteJsonModelParsed> Kdata { get; set; } = new List<ClienteJsonModelParsed>(); // Por defecto un array vacio
    }
}
