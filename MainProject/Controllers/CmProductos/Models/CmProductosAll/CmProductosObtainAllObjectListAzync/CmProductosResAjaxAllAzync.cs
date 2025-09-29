using MainProject.Controllers.CmProductos.Models.CmProductosAll.CmProductosObtainAllObjectListAzync;

namespace MainProject.Controllers.CmProductos.Models.CmProductosAll
{
    public class CmProductosResAjaxAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<CmProductosModelAllJsonParsed> Kdata { get; set; } = new List<CmProductosModelAllJsonParsed>(); // Por defecto un array vacio
    }
}
