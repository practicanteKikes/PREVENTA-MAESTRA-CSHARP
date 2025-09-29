namespace MainProject.Controllers.CustomMunicipios.Models.MunicipiosFiltered.MainGetMunicipiosFilteredAsync
{
    public class FnResMainGetMunicipiosFiltered
    {
        // Required ExternalProvider MINIMAL
        // ADD HERE some properties required by external provider like bancos
        public int Filtered_total { get; set; } = 0;
        public int Response_count { get; set; } = 0;
        public int Response_limited { get; set; } = 0;
        public bool Filtered_has_more { get; set; } = false;
        public string Next_page_url { get; set; } = "";

        public int Response_first_id { get; set; } = 0;
        public int Response_last_id { get; set; } = 0;



        // DEFAULT KIKES DEVELOPERS RESPONSES        


        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ModelMunicipioJsonCustom> Kdata { get; set; } = new List<ModelMunicipioJsonCustom>();
    }
}
