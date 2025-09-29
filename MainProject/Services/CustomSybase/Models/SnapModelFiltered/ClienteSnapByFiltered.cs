using MainProject.Services.CustomSybase.Models.SnapModel;
using SnapObjects.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModelFiltered
{
    // TABLE
    [Table("dba.vw_clientes_full_BIO")]
    //[Table("dba.clientes_full_BIO_tbl")]

    // TOP
    //[SqlSelect("TOP 2 *")] // Add this line to limit to the top 10 results - burned code

    // PARAMS
    [SqlParameter("afteridArgument", typeof(int))]
    [SqlParameter("fecregistroArgument", typeof(string))]
    [SqlParameter("idzonaArgument", typeof(string))]


    // WHERE 
    [SqlWhere("id > :afteridArgument and fec_registro >= :fecregistroArgument AND id_zona = :idzonaArgument")]
    //[SqlWhere("fec_registro >= :fecregistroArgument")]

    // ORDER BY
    [SqlOrderBy("id ASC")]
    //[SqlOrderBy("id ASC, another_colum DESC")]
    public class ClienteSnapByFiltered : ClienteSnapModel
    {
    }
}
