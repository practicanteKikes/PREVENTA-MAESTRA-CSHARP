using MainProject.Services.CustomSybase.Models.SnapModel;

namespace MainProject.Services.CustomSybase.Models.CmMotivosNcQuery.CmMotivosNcQueryAllAzync
{
    public class CmMotivosNcFnResBdQueryAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<CmMotivosNcSnapModel> Kdata { get; set; } = new List<CmMotivosNcSnapModel>(); // Por defecto un array vacio
    }
}
