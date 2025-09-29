using MainProject.Controllers.CmProductos.Models.CmProductosAll.CmProductosMainGetAllAsync;

namespace MainProject.Controllers.CmProductos.Models.CmProductosAll.CmProductosMainGetAllAsync
{
    public class CmProductosFnResMainGetAll
    {
        // Required ExternalProvider MINIMAL
        // ADD HERE some properties required by external provider like bancos



        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<CmProductosModelAllJsonCustom> Kdata { get; set; } = new List<CmProductosModelAllJsonCustom>();
    }
}
