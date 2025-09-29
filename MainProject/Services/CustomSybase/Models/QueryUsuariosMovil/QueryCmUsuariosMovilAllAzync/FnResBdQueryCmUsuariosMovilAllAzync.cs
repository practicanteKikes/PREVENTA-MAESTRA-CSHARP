using MainProject.Services.CustomSybase.Models.SnapModel;

namespace MainProject.Services.CustomSybase.Models.QueryCmUsuariosMovil.All
{
    public class FnResBdQueryCmUsuariosMovilAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<UsuariosMovilSnapModel> Kdata { get; set; } = new List<UsuariosMovilSnapModel>(); // Por defecto un array vacio
    }
}
