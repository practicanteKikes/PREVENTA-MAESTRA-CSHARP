namespace MainProject.Controllers.CustomMunicipios.Models.MunicipiosFiltered.ObtainMunicipiosObjectListAzync
{
    public class FnResObtainMunicipiosObjList
    {
        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84

        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
        public List<ModelMunicipioJsonFiltered> Kdata { get; set; } = new List<ModelMunicipioJsonFiltered>();
    }
}
