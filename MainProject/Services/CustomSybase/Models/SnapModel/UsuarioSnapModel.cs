using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.usuarios")]
    public class UsuarioSnapModel
    {
        //En C#:
        //El tipo de dato decimal es el más adecuado para representar valores monetarios 
        //debido a su precisión exacta.Los tipos de dato float o double pueden introducir errores 
        //de redondeo cuando se realizan operaciones aritméticas con valores monetarios, lo cual no es deseable en aplicaciones financieras.


        //[Key]
        //[Identity]
        [Column("id")] // Define column name from table
        public int Id { get; set; }


        [Column("nombre_usuario")]
        public string? Nombre_usuario { get; set; } = null;


        [Column("e_mail1")]
        public string? Email { get; set; } = null;


        [Column("estado")]
        public string? Estado { get; set; } = null;
    }
}
