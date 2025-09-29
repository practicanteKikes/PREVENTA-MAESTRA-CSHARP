using MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilAll.ObtainCmUsuariosMovilAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilAll.RetrieveDbCmUsuariosMovilAllAzync
{
    public class ResAjaxCmUsuariosMovilAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<ModelCmUsuariosMovilAllJsonParsed> Kdata { get; set; } = new List<ModelCmUsuariosMovilAllJsonParsed>(); // Por defecto un array vacio
    }
}
