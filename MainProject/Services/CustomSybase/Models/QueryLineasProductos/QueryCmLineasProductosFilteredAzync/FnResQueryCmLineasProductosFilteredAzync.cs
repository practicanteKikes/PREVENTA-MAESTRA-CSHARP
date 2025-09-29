using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.QueryCmLineasProductos.Filtered
{
    public class FnResQueryCmLineasProductosFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<LineasProductosFilteredSnapModel> Kdata { get; set; } = new List<LineasProductosFilteredSnapModel>();
    }
}
