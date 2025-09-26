using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.vw_tipos_negocio_B")]
    public class TiposNegocioSnapModel
    {
        //En C#:
        //El tipo de dato decimal es el más adecuado para representar valores monetarios 
        //debido a su precisión exacta.Los tipos de dato float o double pueden introducir errores 
        //de redondeo cuando se realizan operaciones aritméticas con valores monetarios, lo cual no es deseable en aplicaciones financieras.

        [Column("id")] // Define column name from table
        public int Id { get; set; }

        [Column("id_negocio")]
        public string? Id_negocio { get; set; } = null;
        
        [Column("nom_negocio")]
        public string? Nom_negocio { get; set; } = null;
        
        [Column("nota")]
        public string? Nota { get; set; } = null;        

        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;
        
        [Column("fecha")]
        public DateTime? Fecha { get; set; } = null;
    }
}
