using MainProject.Services.CustomSybase.Models.SnapModel;
using SnapObjects.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Services.CustomSybase.Models.SnapModelFiltered
{
    // TABLE
    [Table("dba.vw_municipios_C")]

    // ORDER BY
    [SqlOrderBy("id ASC")]
    public class MunicipiosFilteredSnapModel : MunicipiosSnapModel
    {
    }
}
