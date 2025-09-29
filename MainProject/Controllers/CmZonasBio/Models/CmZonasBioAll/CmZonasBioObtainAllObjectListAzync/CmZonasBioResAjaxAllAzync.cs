using MainProject.Controllers.CmZonasBio.Models.CmZonasBioAll.CmZonasBioObtainAllObjectListAzync;

namespace MainProject.Controllers.CmZonasBio.Models.CmZonasBioAll
{
    public class CmZonasBioResAjaxAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<CmZonasBioModelAllJsonParsed> Kdata { get; set; } = new List<CmZonasBioModelAllJsonParsed>(); // Por defecto un array vacio
    }
}
