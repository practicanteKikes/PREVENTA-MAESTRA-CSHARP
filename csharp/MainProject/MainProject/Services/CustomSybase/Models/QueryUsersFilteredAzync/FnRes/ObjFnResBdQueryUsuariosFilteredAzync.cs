using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.QueryUsersFilteredAzync.FnRes
{
    public class ObjFnResBdQueryUsuariosFilteredAzync
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<UsuarioSnapFilteredCProject> Data { get; set; } = new List<UsuarioSnapFilteredCProject>(); // Por defecto un array vacio
    }
}
