using MainProject.Controllers.CustomDepartamentos.Models.DepartamentosAll.ObtainDepartamentosAllObjectListAzync.JsonParsed;
using MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioAll.ObtainTiposNegocioAllObjectListAzync.JsonParsed;

namespace MainProject.Controllers.CustomDepartamentos.Models.DepartamentosAll.ObtainDepartamentosAllObjectListAzync.FnRes
{
    public class FnResObtainDepartamentosAllObjectList
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ModelDepartamentoAllJsonParsed> Kdata { get; set; } = new List<ModelDepartamentoAllJsonParsed>();
    }
}
