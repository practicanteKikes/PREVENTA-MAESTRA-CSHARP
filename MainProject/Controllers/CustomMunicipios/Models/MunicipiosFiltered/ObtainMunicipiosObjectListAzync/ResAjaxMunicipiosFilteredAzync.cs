namespace MainProject.Controllers.CustomMunicipios.Models.MunicipiosFiltered.ObtainMunicipiosObjectListAzync
{
    public class ResAjaxMunicipiosFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<ModelMunicipioJsonFiltered> Kdata { get; set; } = new List<ModelMunicipioJsonFiltered>();
    }
}
