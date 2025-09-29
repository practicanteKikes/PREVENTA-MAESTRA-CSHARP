using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.CmMotivosNcQuery.CmMotivosNcQueryFilteredAzync
{
    public class CmMotivosNcFnResQueryFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<CmMotivosNcFilteredSnapModel> Kdata { get; set; } = new List<CmMotivosNcFilteredSnapModel>();
    }
}
