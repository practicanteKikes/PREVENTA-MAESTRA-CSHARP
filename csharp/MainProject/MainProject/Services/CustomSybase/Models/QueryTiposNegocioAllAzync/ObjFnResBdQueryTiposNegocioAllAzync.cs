using MainProject.Services.CustomSybase.Models.SnapModel;

namespace MainProject.Services.CustomSybase.Models.QueryTiposNegocioAllAzync
{
    public class ObjFnResBdQueryTiposNegocioAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<TiposNegocioSnapModel> Kdata { get; set; } = new List<TiposNegocioSnapModel>(); // Por defecto un array vacio
    }
}
