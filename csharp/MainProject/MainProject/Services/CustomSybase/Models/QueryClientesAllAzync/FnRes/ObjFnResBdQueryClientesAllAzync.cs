using MainProject.Services.CustomSybase.Models.SnapModel;

namespace MainProject.Services.CustomSybase.Models.QueryClientesAllAzync.FnRes
{
    public class ObjFnResBdQueryClientesAllAzync
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<ClienteSnapModel> Data { get; set; } = new List<ClienteSnapModel>(); // Por defecto un array vacio

    }
}
