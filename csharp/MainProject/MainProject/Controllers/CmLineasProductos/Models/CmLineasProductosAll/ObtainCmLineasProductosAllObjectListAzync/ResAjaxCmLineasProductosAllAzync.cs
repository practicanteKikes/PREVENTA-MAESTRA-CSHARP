using MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosAll.ObtainCmLineasProductosAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosAll.RetrieveDbCmLineasProductosAllAzync
{
    public class ResAjaxCmLineasProductosAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<ModelCmLineasProductosAllJsonParsed> Kdata { get; set; } = new List<ModelCmLineasProductosAllJsonParsed>(); // Por defecto un array vacio
    }
}
