using SnapObjects.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.ca_temp_consignaciones_ws_log")]
    public class RecaudoLogSnapModel
    {
        [Key]
        [Identity]
        [Column("id")] // Define column name from table
        public int Id { get; set; }


        [Column("banco")]
        public string? Banco { get; set; } = null;


        [Column("request_input")]
        public string? Request_input { get; set; } = null;


        [Column("request_output")]
        public string? Request_output { get; set; } = null;


        [Column("status_code")]
        public int? Status_code { get; set; } = null;


        [Column("fec_registro")]
        public DateTime? Fec_registro { get; set; } = null;        


    }
}
