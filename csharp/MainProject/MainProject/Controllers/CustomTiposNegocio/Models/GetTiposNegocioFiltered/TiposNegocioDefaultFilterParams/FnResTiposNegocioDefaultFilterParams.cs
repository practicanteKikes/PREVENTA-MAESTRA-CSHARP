namespace MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.TiposNegocioDefaultFilterParams
{
    public class FnResTiposNegocioDefaultFilterParams
    {
        public int Limit { get; set; }
        public string Fec_registro_min { get; set; }
        public int After_id { get; set; }


        // additional filters
        //public string Zona { get; set; }
    }
}
