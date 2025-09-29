using MainProject.Controllers.CustomClientes.Models.JsonParsed;

namespace MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.RetrieveDbAllClientesFilteredAzync.FnRes
{
    public class ObjFnResBdConsultarClientesFilteredJsonModel
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<ClienteJsonModelFiltered> Data { get; set; } = new List<ClienteJsonModelFiltered>(); // Por defecto un array vacio
    }
}
