using MainProject.Controllers.CustomMunicipios.Models.MunicipiosAll.ObtainMunicipiosAllObjectListAzync.JsonParsed;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.ObtainTiposNegocioAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CustomMunicipios.Models.MunicipiosAll.ObtainMunicipiosAllObjectListAzync.FnRes
{
    public class FnResObtainMunicipiosAllObjectList
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ModelMunicipioAllJsonParsed> Kdata { get; set; } = new List<ModelMunicipioAllJsonParsed>();
    }
}
