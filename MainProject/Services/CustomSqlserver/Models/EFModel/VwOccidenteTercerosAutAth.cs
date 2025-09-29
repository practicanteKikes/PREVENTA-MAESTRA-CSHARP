using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSqlserver.Models.EFModel
{
    // Estoy dudando de si esta anotation es reemplazada en el OnModelCreating de sqlserver EF
    // De todas formas lo dejo porq aparte de EF, se puede usar SnapObjects
    [Table("vw_davivienda_terceros_aut_ath")]
    public partial class VwOccidenteTercerosAutAth
    {
        public int? Id { get; set; }

        [Column("id_banco")]
        public string? IdBanco { get; set; }

        [Column("nit")]
        public string? Nit { get; set; }

        [Column("referencia2")]
        public string? Referencia2 { get; set; }

        [Column("moneda")]
        public string? Moneda { get; set; }

        [Column("valor")]
        public int? Valor { get; set; }

        [Column("fec_registro")]
        public DateTime? FechaRegistro { get; set; }

        [Column("tipo")]
        public string? Tipo { get; set; }
    }
}
