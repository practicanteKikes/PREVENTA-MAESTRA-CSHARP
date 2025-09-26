using MainProject.Controllers.CustomDepartamentos.Models.DepartamentosAll.ObtainDepartamentosAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CustomDepartamentos.Models.DepartamentosAll.RetrieveDbDepartamentosAllAzync
{
    public class ResAjaxDepartamentosAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<ModelDepartamentoAllJsonParsed> Kdata { get; set; } = new List<ModelDepartamentoAllJsonParsed>(); // Por defecto un array vacio
    }
}
