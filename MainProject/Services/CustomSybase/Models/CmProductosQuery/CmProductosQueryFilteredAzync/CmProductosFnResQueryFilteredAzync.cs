using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.CmProductosQuery.CmProductosQueryFilteredAzync
{
    public class CmProductosFnResQueryFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<CmProductosFilteredSnapModel> Kdata { get; set; } = new List<CmProductosFilteredSnapModel>();
    }
}
