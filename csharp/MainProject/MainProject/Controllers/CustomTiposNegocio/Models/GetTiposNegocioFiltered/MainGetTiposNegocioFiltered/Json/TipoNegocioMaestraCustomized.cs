namespace MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.MainGetTiposNegocioFiltered.Json
{
    public class TipoNegocioMaestraCustomized
    {
        // Original columns en la vista
        // [id_negocio,nom_negocio,nota,fec_registro,fecha]


        public int? Id { get; set; } = 1; // Columna aun no implementada en la vista
        public string? Id_negocio { get; set; } = null;
        public string? Nom_negocio { get; set; } = null;
        public string? Nota { get; set; } = null;
        public DateTime? Fec_registro { get; set; } = null;
        public DateTime? Fecha { get; set; } = null;

    }
}
