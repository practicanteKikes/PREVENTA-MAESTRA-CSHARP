using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcFiltered.CmMotivosNcMainGetFilteredAsync
{
    public class CmMotivosNcModelJsonCustom
    {
        public int? Id { get; set; }
        public string? Id_motivo { get; set; } = null;


        public string? Nom_motivo { get; set; } = null;



        public DateTime? Fec_registro { get; set; } = null;


        public DateTime? Fec_act_int { get; set; } = null;
    }
}
