namespace MainProject.Controllers.CmZonasBio.Models.CmZonasBioFiltered.CmZonasBioObtainObjectListAzync
{
    public class CmZonasBioResAjaxFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<CmZonasBioModelJsonFiltered> Kdata { get; set; } = new List<CmZonasBioModelJsonFiltered>();
    }
}
