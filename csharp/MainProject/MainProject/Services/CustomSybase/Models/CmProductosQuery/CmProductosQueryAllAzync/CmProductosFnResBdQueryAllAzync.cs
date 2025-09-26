using MainProject.Services.CustomSybase.Models.SnapModel;

namespace MainProject.Services.CustomSybase.Models.CmProductosQuery.CmProductosQueryAllAzync
{
    public class CmProductosFnResBdQueryAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<CmProductosSnapModel> Kdata { get; set; } = new List<CmProductosSnapModel>(); // Por defecto un array vacio
    }
}
