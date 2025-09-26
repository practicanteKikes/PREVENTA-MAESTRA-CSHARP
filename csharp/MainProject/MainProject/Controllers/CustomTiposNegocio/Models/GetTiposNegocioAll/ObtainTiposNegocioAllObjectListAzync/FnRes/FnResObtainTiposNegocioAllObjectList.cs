using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.ObtainTiposNegocioAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.ObtainTiposNegocioAllObjectListAzync.FnRes
{
    public class FnResObtainTiposNegocioAllObjectList
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<TipoNegocioAllJsonParsed> Kdata { get; set; } = new List<TipoNegocioAllJsonParsed>();
    }
}
