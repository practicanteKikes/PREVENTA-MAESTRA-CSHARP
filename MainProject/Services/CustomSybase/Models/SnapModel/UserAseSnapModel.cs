using SnapObjects.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.usuarios")]
    public class UserAseSnapModel
    {
        [Key]
        //[Identity]
        [Column("id")] // Define column name from table
        public int Id { get; set; }


        [Column("nombre_usuario")]
        public string? User { get; set; } = null;


        [Column("password_usuario")]
        public string? Password { get; set; } = null;


        [Column("e_mail1")]
        public string? Email { get; set; } = null;


        [Column("estado")]
        public string? Estado { get; set; } = null;
    }
}
