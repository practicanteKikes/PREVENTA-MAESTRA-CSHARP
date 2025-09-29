using MainProject.Services.CustomSybase.Models.SnapModel;

namespace MainProject.Services.CustomSybase.Models.QueryDepartamentosAllAzync
{
    public class FnResBdQueryDepartamentosAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<DepartamentosSnapModel> Kdata { get; set; } = new List<DepartamentosSnapModel>(); // Por defecto un array vacio
    }
}
