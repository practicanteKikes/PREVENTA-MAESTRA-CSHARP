using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.vw_usuarios_bionegocios_movil")]
    public class UsuariosMovilSnapModel
    {
        //En C#:
        //El tipo de dato decimal es el más adecuado para representar valores monetarios 
        //debido a su precisión exacta.Los tipos de dato float o double pueden introducir errores 
        //de redondeo cuando se realizan operaciones aritméticas con valores monetarios, lo cual no es deseable en aplicaciones financieras.

        [Column("id")] // Define column name from table
        public int Id { get; set; }

        [Column("id_usuario")]
        public string? Codigo_usuario { get; set; } = null;


        [Column("nombre_usuario")]
        public string? Nombre_completo { get; set; } = null;

        [Column("zona")]
        public string? Zona { get; set; } = null;


        [Column("password_usuario")]
        public string? Password { get; set; } = null;


        [Column("identificacion")]
        public string? Nit { get; set; } = null;


        // Edwin en la vista me entrega los estado activo = true
        // Por tanto oculto la columna estado del query
        //[Column("estado")]
        //public string? Estado { get; set; } = "A"; // Y lo envio quemado por estandar

        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;
    }
}
