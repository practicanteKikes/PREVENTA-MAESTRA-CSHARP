using MainProject.Controllers.CmZonasBio.Models.CmZonasBioAll.CmZonasBioMainGetAllAsync;

namespace MainProject.Controllers.CmZonasBio.Models.CmZonasBioAll.CmZonasBioMainGetAllAsync
{
    public class CmZonasBioFnResMainGetAll
    {
        // Required ExternalProvider MINIMAL
        // ADD HERE some properties required by external provider like bancos



        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<CmZonasBioModelAllJsonCustom> Kdata { get; set; } = new List<CmZonasBioModelAllJsonCustom>();
    }
}
