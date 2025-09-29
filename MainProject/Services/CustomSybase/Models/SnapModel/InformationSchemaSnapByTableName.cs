using SnapObjects.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModel
{
    [Table("sysobjects")]
    [SqlParameter("tablenameArgument", typeof(string))]    

    [SqlWhere("type = 'U' AND name = :tablenameArgument")]
    public class InformationSchemaSnapByTableName : InformationSchemaAseSnapModel
    {
    }
}
