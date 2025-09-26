using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.QueryClientesFilteredAzync.FnRes
{
    public class ObjFnResBdQueryClientesFilteredAzync
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<ClienteSnapByFiltered> Data { get; set; } = new List<ClienteSnapByFiltered>(); // Por defecto un array vacio
    }
}
