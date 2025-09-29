using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.CmZonasBioQuery.CmZonasBioQueryFilteredAzync
{
    public class CmZonasBioFnResQueryFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<CmZonasBioFilteredSnapModel> Kdata { get; set; } = new List<CmZonasBioFilteredSnapModel>();
    }
}
