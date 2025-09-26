using MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosAll.MainGetCmLineasProductosAllAsync.Json;

namespace MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosAll.MainGetCmLineasProductosAllAsync.FnRes
{
    public class FnResMainGetCmLineasProductosAll
    {
        // Required ExternalProvider MINIMAL
        // ADD HERE some properties required by external provider like bancos



        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ModelCmLineasProductosAllJsonCustom> Kdata { get; set; } = new List<ModelCmLineasProductosAllJsonCustom>();
    }
}
