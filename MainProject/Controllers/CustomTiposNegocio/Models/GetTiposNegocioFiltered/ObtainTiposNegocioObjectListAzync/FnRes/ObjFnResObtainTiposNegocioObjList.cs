using MainProject.Services.CustomSybase.Models.SnapModelFiltered;

namespace MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.ObtainTiposNegocioObjectListAzync.FnRes
{
    public class ObjFnResObtainTiposNegocioObjList
    {
        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<TiposNegocioFilteredSnapModel> Kdata { get; set; } = new List<TiposNegocioFilteredSnapModel>();
    }
}
