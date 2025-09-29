using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Controllers.CmZonasBio.Models.CmZonasBioAll.CmZonasBioMainGetAllAsync
{
    public class CmZonasBioModelAllJsonCustom
    {        
        public int? Id { get; set; }

        
        public string? Codigo_bodega { get; set; } = null;

        
        public string? Descripcion { get; set; } = null;


        
        public DateTime? Fec_registro { get; set; } = null;

        
        public string? Id_zona { get; set; } = null;

        
        public string? Estado { get; set; } = null;
    }
}
