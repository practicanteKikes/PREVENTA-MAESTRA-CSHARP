using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.vw_clientes_full_BIO")]
    public class ClienteSnapModel
    {
        //En C#:
        //El tipo de dato decimal es el más adecuado para representar valores monetarios 
        //debido a su precisión exacta.Los tipos de dato float o double pueden introducir errores 
        //de redondeo cuando se realizan operaciones aritméticas con valores monetarios, lo cual no es deseable en aplicaciones financieras.


        //[Key]
        //[Identity]
        [Column("id")] // Define column name from table
        public int Id { get; set; }

        [Column("id_empresa")]
        public string? Id_empresa { get; set; } = null;

        [Column("sucursal")]
        public decimal? Sucursal { get; set; } = null;

        [Column("id_cliente")]
        public string? Id_cliente { get; set; } = null;

        [Column("ind_rut")]
        public string? Ind_rut { get; set; } = null;

        [Column("canal_distribucion")]
        public string? Canal_distribucion { get; set; } = null;

        [Column("estado_cliente")]
        public string? Estado_cliente { get; set; } = null;

        [Column("primer_nombre")]
        public string? Primer_nombre { get; set; } = null;

        [Column("segundo_nombre")]
        public string? Segundo_nombre { get; set; } = null;

        [Column("primer_apellido")]
        public string? Primer_apellido { get; set; } = null;

        [Column("segundo_apellido")]
        public string? Segundo_apellido { get; set; } = null;

        [Column("fecha_cumpleanos")]
        public DateTime? Fecha_cumpleanos { get; set; } = null;

        [Column("nom_negocio")]
        public string? Nom_negocio { get; set; } = null;

        [Column("tipo_negocio")]
        public string? Tipo_negocio { get; set; } = null;

        [Column("direccion")]
        public string? Direccion { get; set; } = null;

        [Column("telefono")]
        public string? Telefono { get; set; } = null;

        [Column("celular")]
        public string? Celular { get; set; } = null;

        [Column("e_mail")]
        public string? E_mail { get; set; } = null;

        [Column("cod_departamento")]
        public string? Cod_departamento { get; set; } = null;

        [Column("id_ciudad")]
        public string? Id_ciudad { get; set; } = null;

        [Column("zona")]
        public string? Zona { get; set; } = null;

        [Column("cod_dias_visita")]
        public string? Cod_dias_visita { get; set; } = null;

        [Column("orden_visita")]
        public decimal? Orden_visita { get; set; } = null;

        [Column("orden_entrega")]
        public decimal? Orden_entrega { get; set; } = null;

        [Column("cod_ruta_distribucion")]
        public string? Cod_ruta_distribucion { get; set; } = null;

        [Column("ind_controlar_cupo")]
        public string? Ind_controlar_cupo { get; set; } = null;

        [Column("cupo")]
        public decimal? Cupo { get; set; } = null;

        [Column("saldo")]
        public decimal? Saldo { get; set; } = null;

        [Column("anticipos")]
        public decimal? Anticipos { get; set; } = null;

        [Column("nit_alterno")]
        public string? Nit_alterno { get; set; } = null;

        [Column("nombre_alterno")]
        public string? Nombre_alterno { get; set; } = null;

        [Column("apellidos_alterno")]
        public string? Apellidos_alterno { get; set; } = null;

        [Column("plazo_factura")]
        public int? Plazo_factura { get; set; } = null;

        [Column("num_factura_cartera")]
        public decimal? Num_factura_cartera { get; set; } = null;

        [Column("ind_gran_contribuyente")]
        public string? Ind_gran_contribuyente { get; set; } = null;

        [Column("ind_autoretenedor")]
        public string? Ind_autoretenedor { get; set; } = null;

        [Column("resolucion_retencion_fuente")]
        public string? Resolucion_retencion_fuente { get; set; } = null;

        [Column("ind_agente_retencion_renta")]
        public string? Ind_agente_retencion_renta { get; set; } = null;

        [Column("ind_facturar_iva")]
        public string? Ind_facturar_iva { get; set; } = null;

        [Column("regimen_iva")]
        public string? Regimen_iva { get; set; } = null;

        [Column("nota1")]
        public string? Nota1 { get; set; } = null;

        [Column("fecha_ingreso")]
        public DateTime? Fecha_ingreso { get; set; } = null;

        [Column("fecha_factura")]
        public DateTime? Fecha_factura { get; set; } = null;

        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;

        [Column("ind_modificado")]
        public string? Ind_modificado { get; set; } = null;

        [Column("naturaleza")]
        public string? Naturaleza { get; set; } = null;

        [Column("razon_social")]
        public string? Razon_social { get; set; } = null;

        [Column("id_zona_facturacion")]
        public string? Id_zona_facturacion { get; set; } = null;

        [Column("ind_masivo")]
        public string? Ind_masivo { get; set; } = null;

        [Column("id_categoria_cliente")]
        public string? Id_categoria_cliente { get; set; } = null;

        [Column("orden_visita_lunes")]
        public decimal? Orden_visita_lunes { get; set; } = null;

        [Column("orden_visita_martes")]
        public decimal? Orden_visita_martes { get; set; } = null;

        [Column("orden_visita_miercoles")]
        public decimal? Orden_visita_miercoles { get; set; } = null;

        [Column("orden_visita_jueves")]
        public decimal? Orden_visita_jueves { get; set; } = null;

        [Column("orden_visita_viernes")]
        public decimal? Orden_visita_viernes { get; set; } = null;

        [Column("orden_visita_sabado")]
        public decimal? Orden_visita_sabado { get; set; } = null;

        [Column("orden_visita_domingo")]
        public decimal? Orden_visita_domingo { get; set; } = null;

        [Column("num_impresion_original")]
        public string? Num_impresion_original { get; set; } = null;

        [Column("Ind_entrega_certificada")]
        public string? Ind_entrega_certificada { get; set; } = null;

        [Column("valor_latitud")]
        public string? Valor_latitud { get; set; } = null;

        [Column("valor_longitud")]
        public string? Valor_longitud { get; set; } = null;

        [Column("porcentaje_rotura")]
        public string? Porcentaje_rotura { get; set; } = null;

        [Column("tipo_cliente")]
        public string? Tipo_cliente { get; set; } = null;


        
        [Column("correo_factu_electronica")]
        public string? Correo_factu_electronica { get; set; } = null;
    }
}
