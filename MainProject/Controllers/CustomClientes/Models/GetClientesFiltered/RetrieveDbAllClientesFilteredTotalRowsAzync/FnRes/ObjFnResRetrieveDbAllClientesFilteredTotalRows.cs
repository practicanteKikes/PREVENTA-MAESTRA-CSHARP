using MainProject.Services.CustomDatabaseInput.Models.Json;

namespace MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.RetrieveDbAllClientesFilteredTotalRowsAzync.FnRes
{
    public class ObjFnResRetrieveDbAllClientesFilteredTotalRows
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<int> Data { get; set; } = new List<int>(); // Por defecto un array vacio
    }
}
