using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.QueryCmUsuariosMovil.Filtered
{
    public class FnResQueryCmUsuariosMovilFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<UsuariosMovilFilteredSnapModel> Kdata { get; set; } = new List<UsuariosMovilFilteredSnapModel>();
    }
}
