namespace MainProject.Controllers.CmZonasBio.Models.CmZonasBioFiltered.CmZonasBioDefaultFilterParams
{
    public class CmZonasBioFnResDefaultFilterParams
    {
        internal object id_zona;

        public int Limit { get; set; }
        public string Fec_registro_min { get; set; }
        public int After_id { get; set; }


        // additional filters
        public string id_Zona { get; set; }
    }
}
