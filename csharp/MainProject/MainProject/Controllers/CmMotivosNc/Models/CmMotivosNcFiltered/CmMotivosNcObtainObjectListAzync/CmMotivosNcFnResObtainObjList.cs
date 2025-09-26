namespace MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcFiltered.CmMotivosNcObtainObjectListAzync
{
    public class CmMotivosNcFnResObtainObjList
    {
        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84

        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<CmMotivosNcModelJsonFiltered> Kdata { get; set; } = new List<CmMotivosNcModelJsonFiltered>();
    }
}
