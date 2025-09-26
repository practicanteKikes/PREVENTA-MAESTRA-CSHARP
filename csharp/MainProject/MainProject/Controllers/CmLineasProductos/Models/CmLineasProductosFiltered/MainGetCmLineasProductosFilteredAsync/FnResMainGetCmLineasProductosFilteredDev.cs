namespace MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosFiltered.MainGetCmLineasProductosFilteredAsync
{
    public class FnResMainGetCmLineasProductosFilteredDev : FnResMainGetCmLineasProductosFiltered
    {
        // EXTRA INFO        
        public string Db_name_input { get; set; } = "";
        public string Db_name_output { get; set; } = "";
        public Dictionary<string, object> Kfiltered_params { get; set; } = new Dictionary<string, object>();
    }
}
