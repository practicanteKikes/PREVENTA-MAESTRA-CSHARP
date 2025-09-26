using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.MainGetTiposNegocioAllAsync.Json;

namespace MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.MainGetTiposNegocioAllAsync.FnRes
{
    public class FnResMainGetTiposNegocioAll
    {
        // Required ExternalProvider MINIMAL
        // ADD HERE some properties required by external provider like bancos



        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<TipoNegocioAllMaestraCustomized> Kdata { get; set; } = new List<TipoNegocioAllMaestraCustomized>();
    }
}
