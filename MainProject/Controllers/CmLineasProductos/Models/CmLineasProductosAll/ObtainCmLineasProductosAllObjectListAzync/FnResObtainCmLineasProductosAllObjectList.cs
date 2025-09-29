using MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosAll.ObtainCmLineasProductosAllObjectListAzync.JsonParsed;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.ObtainTiposNegocioAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosAll.ObtainCmLineasProductosAllObjectListAzync.FnRes
{
    public class FnResObtainCmLineasProductosAllObjectList
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ModelCmLineasProductosAllJsonParsed> Kdata { get; set; } = new List<ModelCmLineasProductosAllJsonParsed>();
    }
}
