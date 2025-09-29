using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("sysobjects")]
    public class InformationSchemaAseSnapModel
    {        
        [Column("type")]
        public string? Type { get; set; } = null;

        [Column("name")]
        public string? Name { get; set; } = null;
    }
}
