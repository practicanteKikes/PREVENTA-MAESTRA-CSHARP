namespace MainProject.Controllers.CustomTiposNegocio.Models.GetTiposNegocioFiltered.ObtainTiposNegocioObjectListAzync.JsonParsed
{
    public class TipoNegocioJsonModelFiltered
    {
        public int? Id { get; set; } = null;

        public string? Id_empresa { get; set; } = null;

        public decimal? Sucursal { get; set; } = null;

        public string? Id_cliente { get; set; } = null;

        public string? Ind_rut { get; set; } = null;

        public string? Canal_distribucion { get; set; } = null;

        public string? Estado_cliente { get; set; } = null;

        public string? Primer_nombre { get; set; } = null;

        public string? Segundo_nombre { get; set; } = null;

        public string? Primer_apellido { get; set; } = null;

        public string? Segundo_apellido { get; set; } = null;

        public DateTime? Fecha_cumpleanos { get; set; } = null;

        public string? Nom_negocio { get; set; } = null;

        public string? Tipo_negocio { get; set; } = null;

        public string? Direccion { get; set; } = null;

        public string? Telefono { get; set; } = null;

        public string? Celular { get; set; } = null;

        public string? E_mail { get; set; } = null;

        public string? Cod_departamento { get; set; } = null;

        public string? Id_ciudad { get; set; } = null;

        public string? Zona { get; set; } = null;

        public string? Cod_dias_visita { get; set; } = null;

        public decimal? Orden_visita { get; set; } = null;

        public decimal? Orden_entrega { get; set; } = null;

        public string? Cod_ruta_distribucion { get; set; } = null;

        public string? Ind_controlar_cupo { get; set; } = null;

        public decimal? Cupo { get; set; } = null;

        public decimal? Saldo { get; set; } = null;

        public decimal? Anticipos { get; set; } = null;

        public string? Nit_alterno { get; set; } = null;

        public string? Nombre_alterno { get; set; } = null;

        public string? Apellidos_alterno { get; set; } = null;

        public int? Plazo_factura { get; set; } = null;

        public decimal? Num_factura_cartera { get; set; } = null;

        public string? Ind_gran_contribuyente { get; set; } = null;

        public string? Ind_autoretenedor { get; set; } = null;

        public string? Resolucion_retencion_fuente { get; set; } = null;

        public string? Ind_agente_retencion_renta { get; set; } = null;

        public string? Ind_facturar_iva { get; set; } = null;

        public string? Regimen_iva { get; set; } = null;

        public string? Nota1 { get; set; } = null;

        public DateTime? Fecha_ingreso { get; set; } = null;

        public DateTime? Fecha_factura { get; set; } = null;

        public DateTime? Fec_registro { get; set; } = null;

        public string? Ind_modificado { get; set; } = null;

        public string? Naturaleza { get; set; } = null;

        public string? Razon_social { get; set; } = null;

        public string? Id_zona_facturacion { get; set; } = null;

        public string? Ind_masivo { get; set; } = null;

        public string? Id_categoria_cliente { get; set; } = null;

        public decimal? Orden_visita_lunes { get; set; } = null;

        public decimal? Orden_visita_martes { get; set; } = null;

        public decimal? Orden_visita_miercoles { get; set; } = null;

        public decimal? Orden_visita_jueves { get; set; } = null;

        public decimal? Orden_visita_viernes { get; set; } = null;

        public decimal? Orden_visita_sabado { get; set; } = null;

        public decimal? Orden_visita_domingo { get; set; } = null;

        public string? Num_impresion_original { get; set; } = null;

        public string? Ind_entrega_certificada { get; set; } = null;

        public string? Valor_latitud { get; set; } = null;

        public string? Valor_longitud { get; set; } = null;

        public string? Porcentaje_rotura { get; set; } = null;

        public string? Tipo_cliente { get; set; } = null;



        // custom csharp developer logic properties
        const string ACTIVECHARACTER = "A";

        // custom csharp developer logic methods
        public bool IsActive()
        {
            if (Estado_cliente == ACTIVECHARACTER)
            {
                return true;
            }
            return false;
        }
    }
}
