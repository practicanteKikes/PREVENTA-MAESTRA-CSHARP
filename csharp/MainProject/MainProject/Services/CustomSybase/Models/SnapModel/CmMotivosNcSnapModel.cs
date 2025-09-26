using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.vw_motivos_no_venta_BIO")]
    public class CmMotivosNcSnapModel
    {
        //En C#:
        //El tipo de dato decimal es el más adecuado para representar valores monetarios 
        //debido a su precisión exacta.Los tipos de dato float o double pueden introducir errores 
        //de redondeo cuando se realizan operaciones aritméticas con valores monetarios, lo cual no es deseable en aplicaciones financieras.

        [Column("id")] // Define column name from table
        public int Id { get; set; }

        [Column("id_motivo")]
        public string? Id_motivo { get; set; } = null;

        [Column("nom_motivo")]
        public string? Nom_motivo { get; set; } = null;


        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;

        [Column("fec_act_int")]
        public DateTime? Fec_act_int { get; set; } = null;
    }
}
