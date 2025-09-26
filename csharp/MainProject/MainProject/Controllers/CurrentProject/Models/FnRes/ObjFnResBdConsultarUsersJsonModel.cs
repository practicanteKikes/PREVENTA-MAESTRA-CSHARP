using MainProject.Controllers.CustomClientes.Models.JsonParsed;

namespace MainProject.Controllers.CurrentProject.Models.FnRes
{
    public class ObjFnResBdConsultarUsersJsonModel
    {
        // Respuesta de api bd
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<ClienteJsonModelFiltered> Data { get; set; } = new List<ClienteJsonModelFiltered>(); // Por defecto un array vacio
    }
}
