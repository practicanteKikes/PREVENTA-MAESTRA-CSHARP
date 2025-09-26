using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.vw_productos_BIO")]    
    public class CmProductosSnapModel
    {
        //En C#:
        //El tipo de dato decimal es el más adecuado para representar valores monetarios 
        //debido a su precisión exacta.Los tipos de dato float o double pueden introducir errores 
        //de redondeo cuando se realizan operaciones aritméticas con valores monetarios, lo cual no es deseable en aplicaciones financieras.

        [Column("id")] // Define column name from table
        public int Id { get; set; }

        [Column("cod_prod")]
        public string? Cod_prod { get; set; } = null;

        [Column("codigo_linea")]
        public string? Codigo_linea { get; set; } = null;

        [Column("cod_prod2")]
        public string? Cod_prod2 { get; set; } = null;

        [Column("nom_prod")]
        public string? Nom_prod { get; set; } = null;

        [Column("nombre_corto")]
        public string? Nombre_corto { get; set; } = null;

        [Column("orden")]
        public Decimal? Orden { get; set; } = null;


        [Column("estado")]
        public string? Estado { get; set; } = null;


        [Column("por_iva")]
        public Decimal? Por_iva { get; set; } = null;

        [Column("cod_iva")]
        public string? Cod_iva { get; set; } = null;

        [Column("porc_rte")]
        public Decimal? Porc_rte { get; set; } = null;

        [Column("cod_rte")]
        public string? Cod_rte { get; set; } = null;

        [Column("embalaje")]
        public Decimal? Embalaje { get; set; } = null;


        [Column("obliga")]
        public string? Obliga { get; set; } = null;

        [Column("descarga")]
        public string? Descarga { get; set; } = null;

        [Column("unidad_medida")]
        public string? Unidad_medida { get; set; } = null;

        [Column("cod_sublinea")]
        public string? Cod_sublinea { get; set; } = null;

        [Column("nom_sublinea")]
        public string? Nom_sublinea { get; set; } = null;

        [Column("permite_decimales")]
        public string? Permite_decimales { get; set; } = null;

        [Column("tipo_operacion")]
        public string? Tipo_operacion { get; set; } = null;


        [Column("unidades_x_canasta")]
        public Decimal? Unidades_x_canasta { get; set; } = null;





        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;

        [Column("tipo_embalaje")]
        public string? Tipo_embalaje { get; set; } = null;

        [Column("fecha")]
        public DateTime? Fecha { get; set; } = null;
    }
}
