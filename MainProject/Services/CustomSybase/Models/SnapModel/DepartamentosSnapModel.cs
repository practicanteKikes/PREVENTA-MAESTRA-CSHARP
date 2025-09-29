using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.vw_departamentos_C")]
    public class DepartamentosSnapModel
    {
        //En C#:
        //El tipo de dato decimal es el más adecuado para representar valores monetarios 
        //debido a su precisión exacta.Los tipos de dato float o double pueden introducir errores 
        //de redondeo cuando se realizan operaciones aritméticas con valores monetarios, lo cual no es deseable en aplicaciones financieras.

        [Column("id")] // Define column name from table
        public int Id { get; set; }

        [Column("id_departamento")]
        public string? Id_departamento { get; set; } = null;

        [Column("descripcion")]
        public string? Descripcion { get; set; } = null;        

        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;

        [Column("fecha")]
        public DateTime? Fecha { get; set; } = null;
    }
}
