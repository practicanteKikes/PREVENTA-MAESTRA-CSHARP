using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSqlserver.Models.EFModel
{
    [Table("BANCOS_CONVENIOS")]
    public class BancoConvenioEF
    {
        // [Column("id")]
        public int Id { get; set; } // never null - is autoincrement

        [Column("cod_banconal")]
        public string? Cod_banconal { get; set; } = null;

        [Column("descripcion")]
        public string? Descripcion { get; set; } = null;

        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;

        [Column("cuenta_banco")]
        public string? Cuenta_banco { get; set; } = null;

        [Column("cod_convenio_recaudo")]
        public string? Cod_convenio_recaudo { get; set; } = null;

        [Column("fecha")]
        public DateTime? Fecha { get; set; } = null;
    }
}
