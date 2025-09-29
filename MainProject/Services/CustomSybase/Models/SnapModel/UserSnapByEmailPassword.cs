using SnapObjects.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("dba.usuarios")]
    [SqlParameter("emailArgument", typeof(string))]
    [SqlParameter("passwordArgument", typeof(string))]

    [SqlWhere("e_mail1 = :emailArgument AND password_usuario = :passwordArgument")]
    public class UserSnapByEmailPassword : UserAseSnapModel
    {

    }
}
