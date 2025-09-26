using MainProject.Services.CustomSybase.Models.SnapModel;
using SnapObjects.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModelFiltered
{
    // TABLE
    [Table("dba.vw_motivos_no_venta_BIO")]

    // ORDER BY
    [SqlOrderBy("id ASC")]
    public class CmMotivosNcFilteredSnapModel : CmMotivosNcSnapModel
    {
    }
}