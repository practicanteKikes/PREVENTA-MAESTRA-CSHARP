using MainProject.Services.CustomSybase.Models.SnapModel;

namespace MainProject.Services.CustomSybase.Models.QueryMunicipiosAllAzync
{
    public class FnResBdQueryMunicipiosAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<MunicipiosSnapModel> Kdata { get; set; } = new List<MunicipiosSnapModel>(); // Por defecto un array vacio
    }
}
