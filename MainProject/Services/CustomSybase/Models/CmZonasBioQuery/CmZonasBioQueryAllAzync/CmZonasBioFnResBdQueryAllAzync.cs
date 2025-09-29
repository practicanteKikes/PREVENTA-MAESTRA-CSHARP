using MainProject.Services.CustomSybase.Models.SnapModel;

namespace MainProject.Services.CustomSybase.Models.CmZonasBioQuery.CmZonasBioQueryAllAzync
{
    public class CmZonasBioFnResBdQueryAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<CmZonasBioSnapModel> Kdata { get; set; } = new List<CmZonasBioSnapModel>(); // Por defecto un array vacio
    }
}
