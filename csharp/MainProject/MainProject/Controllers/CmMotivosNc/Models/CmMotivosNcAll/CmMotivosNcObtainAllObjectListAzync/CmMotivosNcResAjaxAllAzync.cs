using MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcAll.CmMotivosNcObtainAllObjectListAzync;

namespace MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcAll
{
    public class CmMotivosNcResAjaxAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<CmMotivosNcModelAllJsonParsed> Kdata { get; set; } = new List<CmMotivosNcModelAllJsonParsed>(); // Por defecto un array vacio
    }
}
