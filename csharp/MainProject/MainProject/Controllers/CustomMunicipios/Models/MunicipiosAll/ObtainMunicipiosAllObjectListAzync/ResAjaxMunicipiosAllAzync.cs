using MainProject.Controllers.CustomMunicipios.Models.MunicipiosAll.ObtainMunicipiosAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CustomMunicipios.Models.MunicipiosAll.RetrieveDbMunicipiosAllAzync
{
    public class ResAjaxMunicipiosAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<ModelMunicipioAllJsonParsed> Kdata { get; set; } = new List<ModelMunicipioAllJsonParsed>(); // Por defecto un array vacio
    }
}
