using MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilAll.MainGetCmUsuariosMovilAllAsync.Json;

namespace MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilAll.MainGetCmUsuariosMovilAllAsync.FnRes
{
    public class FnResMainGetCmUsuariosMovilAll
    {
        // Required ExternalProvider MINIMAL
        // ADD HERE some properties required by external provider like bancos



        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public DateTime? Kldt_fecha_servidor_ase { get; set; } = null;

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ModelCmUsuariosMovilAllJsonCustom> Kdata { get; set; } = new List<ModelCmUsuariosMovilAllJsonCustom>();
    }
}
