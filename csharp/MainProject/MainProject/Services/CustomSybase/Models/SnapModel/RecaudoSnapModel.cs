using SnapObjects.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.ca_temp_consignaciones_ws")]
    public class RecaudoSnapModel
    {
        [Key]
        [Identity]
        [Column("id")] // Define column name from table
        public int Id { get; set; }

        [Column("id_empresa")]
        public string? Id_empresa { get; set; } = null;

        [Column("id_cliente")]
        public string? Id_cliente { get; set; } = null;


        // Numeric Sybase are considered decimal in csharp
        [Column("contador")]
        public decimal? Contador { get; set; } = null;

        [Column("nombre_cliente")]
        public string? Nombre_cliente { get; set; } = null;

        [Column("fecha_consignacion")]
        public DateTime? Fecha_consignacion { get; set; } = null;

        [Column("banco")]
        public string? Banco { get; set; } = null;

        [Column("num_cuenta")]
        public string? Num_cuenta { get; set; } = null;

        [Column("colilla_ref")]
        public string? Colilla_ref { get; set; } = null;

        [Column("valor")]
        public decimal? Valor { get; set; } = null;

        [Column("id_usuario")]
        public string? Id_usuario { get; set; } = null;

        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;

        [Column("canal_recaudo")]
        public string? Canal_recaudo { get; set; } = null;

        [Column("cod_convenio")]
        public string? Cod_convenio { get; set; } = null;

        [Column("forma_pago")]
        public string? Forma_pago { get; set; } = null;

        [Column("hora_consignacion")]
        public DateTime? Hora_consignacion { get; set; } = null;

        [Column("jornada_recaudo")]
        public string? Jornada_recaudo { get; set; } = null;

        [Column("num_cheque")]
        public string? Num_cheque { get; set; } = null;

        [Column("num_terminal")]
        public string? Num_terminal { get; set; } = null;

        [Column("tipo_canje")]
        public string? Tipo_canje { get; set; } = null;

        [Column("tipo_moneda")]
        public string? Tipo_moneda { get; set; } = null;

        [Column("valor_cheque")]
        public decimal? Valor_cheque { get; set; } = null;

        [Column("valor_efectivo")]
        public decimal? Valor_efectivo { get; set; } = null;

        [Column("fecha_colilla")]
        public DateTime? Fecha_colilla { get; set; } = null;

        [Column("ind_ruta_distribucion")]
        public string? Ind_ruta_distribucion { get; set; } = null;

        [Column("tipo_negocio")]
        public string? Tipo_negocio { get; set; } = null;

        [Column("nombre_tipo_negocio_cliente")]
        public string? Nombre_tipo_negocio_cliente { get; set; } = null;

        [Column("observaciones")]
        public string? Observaciones { get; set; } = null;

        [Column("radicado_bancario")]
        public string? Radicado_bancario { get; set; } = null;

        [Column("json_bancario")]
        public string? Json_bancario { get; set; } = null;

        [Column("estado")]
        public string? Estado { get; set; } = null;
    }
}
