using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.ObtainTiposNegocioAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.RetrieveDbTiposNegocioAllAzync
{
    public class FnResRetrieveDbTiposNegocioAllAzync
    {
        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<TipoNegocioAllJsonParsed> Kdata { get; set; } = new List<TipoNegocioAllJsonParsed>(); // Por defecto un array vacio
    }
}
