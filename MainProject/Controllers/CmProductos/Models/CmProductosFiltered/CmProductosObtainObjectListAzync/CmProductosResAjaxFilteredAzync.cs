namespace MainProject.Controllers.CmProductos.Models.CmProductosFiltered.CmProductosObtainObjectListAzync
{
    public class CmProductosResAjaxFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<CmProductosModelJsonFiltered> Kdata { get; set; } = new List<CmProductosModelJsonFiltered>();
    }
}
