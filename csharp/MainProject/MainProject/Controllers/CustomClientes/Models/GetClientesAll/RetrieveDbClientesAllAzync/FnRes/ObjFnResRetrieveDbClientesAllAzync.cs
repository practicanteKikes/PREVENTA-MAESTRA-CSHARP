using MainProject.Controllers.CustomClientes.Models.JsonParsed;

namespace MainProject.Controllers.CustomClientes.Models.GetClientesAll.RetrieveDbClientesAllAzync.FnRes
{
    public class ObjFnResRetrieveDbClientesAllAzync
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<ClienteJsonModelParsed> Data { get; set; } = new List<ClienteJsonModelParsed>(); // Por defecto un array vacio
    }
}
