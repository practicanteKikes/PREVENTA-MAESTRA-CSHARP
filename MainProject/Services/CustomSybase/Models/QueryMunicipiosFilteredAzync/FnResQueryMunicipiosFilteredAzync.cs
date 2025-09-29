using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.QueryMunicipiosFilteredAzync
{
    public class FnResQueryMunicipiosFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<MunicipiosFilteredSnapModel> Kdata { get; set; } = new List<MunicipiosFilteredSnapModel>();

    }
}
