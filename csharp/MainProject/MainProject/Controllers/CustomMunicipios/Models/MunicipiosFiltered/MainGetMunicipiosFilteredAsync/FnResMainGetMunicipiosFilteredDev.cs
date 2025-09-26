namespace MainProject.Controllers.CustomMunicipios.Models.MunicipiosFiltered.MainGetMunicipiosFilteredAsync
{
    public class FnResMainGetMunicipiosFilteredDev : FnResMainGetMunicipiosFiltered
    {
        // EXTRA INFO        
        public string Db_name_input { get; set; } = "";
        public string Db_name_output { get; set; } = "";
        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
    }
}
