using MainProject.Services.CustomDatabaseInput.Models.Json;

namespace MainProject.Controllers.CustomClientes.Models.GetClientesFiltered.ObtainTotalRowsFilteredWithOutLimitClientes.FnRes
{
    public class ObjFnResObtainTotalRowsFilteredWithOutLimitedClientes
    {
        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<int> Kdata { get; set; } = new List<int>();
    }
}
