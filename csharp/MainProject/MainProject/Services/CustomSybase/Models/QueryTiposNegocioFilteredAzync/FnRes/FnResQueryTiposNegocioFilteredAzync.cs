using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.QueryTiposNegocioFilteredAzync.FnRes
{
    public class FnResQueryTiposNegocioFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<TiposNegocioFilteredSnapModel> Kdata { get; set; } = new List<TiposNegocioFilteredSnapModel>();
    }
}
