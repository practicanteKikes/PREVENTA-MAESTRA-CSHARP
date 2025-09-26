using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.vw_lineas_productos_BIO")]
    public class LineasProductosSnapModel
    {
        //En C#:
        //El tipo de dato decimal es el más adecuado para representar valores monetarios 
        //debido a su precisión exacta.Los tipos de dato float o double pueden introducir errores 
        //de redondeo cuando se realizan operaciones aritméticas con valores monetarios, lo cual no es deseable en aplicaciones financieras.

        [Column("id")] // Define column name from table
        public int Id { get; set; }

        [Column("codigo_linea")]
        public string? Codigo_linea { get; set; } = null;


        [Column("nombre")]
        public string? Nombre { get; set; } = null;


        // Edwin en la vista me entrega los estado activo = true
        // Por tanto oculto la columna estado del query
        //[Column("estado")]
        //public string? Estado { get; set; } = "A"; // Y lo envio quemado por estandar

        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;

        [Column("fecha")]
        public DateTime? Fecha { get; set; } = null;
    }
}
