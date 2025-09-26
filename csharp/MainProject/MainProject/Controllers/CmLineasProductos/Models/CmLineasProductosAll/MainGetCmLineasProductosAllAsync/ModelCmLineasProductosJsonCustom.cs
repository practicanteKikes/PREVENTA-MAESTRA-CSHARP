namespace MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosAll.MainGetCmLineasProductosAllAsync.Json
{
    public class ModelCmLineasProductosAllJsonCustom
    {
        public int? Id { get; set; }
        public string? Codigo_linea { get; set; } = null;
        public string? Nombre { get; set; } = null;

        //public string? Estado { get; set; } = "A"; // Por defecto la vista no envia esta columna, pero vienen los que están activos

        public DateTime? Fec_registro { get; set; } = null;
        public DateTime? Fecha { get; set; } = null;
    }
}
