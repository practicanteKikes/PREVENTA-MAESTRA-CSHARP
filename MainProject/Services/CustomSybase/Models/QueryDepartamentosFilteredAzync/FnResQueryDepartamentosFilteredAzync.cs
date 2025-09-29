using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Services.CustomSybase.Models.QueryDepartamentosFilteredAzync
{
    public class FnResQueryDepartamentosFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<DepartamentosFilteredSnapModel> Kdata { get; set; } = new List<DepartamentosFilteredSnapModel>();
    }
}
