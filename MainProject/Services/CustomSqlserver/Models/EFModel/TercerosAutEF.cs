using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSqlserver.Models.EFModel
{
    [Table("TERCEROS_AUT")]

    public class TercerosAutEF
    {
        public int Id { get; set; }

        [Column("id_cliente")]
        public string IdCliente { get; set; } = "";

        [Column("descripcion")]
        public string Descripcion { get; set; } = "";

        [Column("estado")]
        public string Estado { get; set; } = "";

        [Column("fec_registro")]
        public string FecRegistro { get; set; } = "";
    }
}
