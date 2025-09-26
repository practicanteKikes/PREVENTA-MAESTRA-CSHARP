namespace MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcFiltered.CmMotivosNcObtainObjectListAzync
{
    public class CmMotivosNcResAjaxFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<CmMotivosNcModelJsonFiltered> Kdata { get; set; } = new List<CmMotivosNcModelJsonFiltered>();
    }
}
