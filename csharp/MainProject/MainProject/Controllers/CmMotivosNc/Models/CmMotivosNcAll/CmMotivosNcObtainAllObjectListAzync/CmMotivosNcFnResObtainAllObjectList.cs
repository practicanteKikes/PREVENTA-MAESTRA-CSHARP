using MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcAll.CmMotivosNcObtainAllObjectListAzync;

namespace MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcAll.CmMotivosNcObtainAllObjectListAzync
{
    public class CmMotivosNcFnResObtainAllObjectList
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<CmMotivosNcModelAllJsonParsed> Kdata { get; set; } = new List<CmMotivosNcModelAllJsonParsed>();
    }
}
