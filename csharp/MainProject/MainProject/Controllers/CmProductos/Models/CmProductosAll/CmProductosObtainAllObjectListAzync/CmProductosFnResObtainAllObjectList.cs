using MainProject.Controllers.CmProductos.Models.CmProductosAll.CmProductosObtainAllObjectListAzync;

namespace MainProject.Controllers.CmProductos.Models.CmProductosAll.CmProductosObtainAllObjectListAzync
{
    public class CmProductosFnResObtainAllObjectList
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<CmProductosModelAllJsonParsed> Kdata { get; set; } = new List<CmProductosModelAllJsonParsed>();
    }
}
