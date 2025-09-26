using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Controllers.CmLineasProductos.Models.CmLineasProductosFiltered.ObtainCmLineasProductosObjectListAzync{
    public class ModelCmLineasProductosJsonFiltered
    {        
        public int Id { get; set; }
        public string? Codigo_linea { get; set; } = null;

        public string? Nombre { get; set; } = null;
        
        //public string? Estado { get; set; } = null;
        public DateTime? Fec_registro { get; set; } = null;

        public DateTime? Fecha { get; set; } = null;
    }
}
