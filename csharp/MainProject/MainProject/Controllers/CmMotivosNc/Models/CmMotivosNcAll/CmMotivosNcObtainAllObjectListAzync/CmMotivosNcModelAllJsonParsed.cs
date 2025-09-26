using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcAll.CmMotivosNcObtainAllObjectListAzync
{
    public class CmMotivosNcModelAllJsonParsed
    {
        public int? Id { get; set; }
        
        public string? Id_motivo { get; set; } = null;

        
        public string? Nom_motivo { get; set; } = null;


        
        public DateTime? Fec_registro { get; set; } = null;

        
        public DateTime? Fec_act_int { get; set; } = null;
    }
}
