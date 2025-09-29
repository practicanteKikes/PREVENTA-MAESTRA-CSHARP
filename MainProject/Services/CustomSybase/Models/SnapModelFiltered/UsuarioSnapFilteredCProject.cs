using MainProject.Services.CustomSybase.Models.SnapModel;
using SnapObjects.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModelFiltered
{
    // TABLE
    [Table("dba.usuarios")]

    // PARAMS
    //[SqlParameter("afteridArgument", typeof(int))]
    //[SqlParameter("fecregistroArgument", typeof(string))]

    // WHERE 
    //[SqlWhere("id > :afteridArgument")]


    // ORDER BY
    [SqlOrderBy("id ASC")]
    //[SqlOrderBy("id ASC, another_colum DESC")]
    public class UsuarioSnapFilteredCProject : UsuarioSnapModel
    {
    }
}
