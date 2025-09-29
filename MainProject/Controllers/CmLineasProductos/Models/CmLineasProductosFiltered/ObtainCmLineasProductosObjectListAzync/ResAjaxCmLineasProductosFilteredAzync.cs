namespace MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosFiltered.ObtainCmLineasProductosObjectListAzync
{
    public class ResAjaxCmLineasProductosFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<ModelCmLineasProductosJsonFiltered> Kdata { get; set; } = new List<ModelCmLineasProductosJsonFiltered>();
    }
}
