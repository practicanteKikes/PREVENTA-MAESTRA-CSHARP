using MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilAll.ObtainCmUsuariosMovilAllObjectListAzync.JsonParsed;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.ObtainTiposNegocioAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilAll.ObtainCmUsuariosMovilAllObjectListAzync.FnRes
{
    public class FnResObtainCmUsuariosMovilAllObjectList
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ModelCmUsuariosMovilAllJsonParsed> Kdata { get; set; } = new List<ModelCmUsuariosMovilAllJsonParsed>();
    }
}
