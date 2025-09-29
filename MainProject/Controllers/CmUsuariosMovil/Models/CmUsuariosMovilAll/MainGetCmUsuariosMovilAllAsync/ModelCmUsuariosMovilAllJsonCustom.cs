namespace MainProject.Controllers.CmUsuariosMovil.Models.CmUsuariosMovilAll.MainGetCmUsuariosMovilAllAsync.Json
{
    public class ModelCmUsuariosMovilAllJsonCustom
    {        
        public int? Id { get; set; }
        public string? Codigo_usuario { get; set; } = null;
        public string? Nombre_completo { get; set; } = null;
        public string? Zona { get; set; } = null;

        public string? Password { get; set; } = null;

        public string? Nit { get; set; } = null;

        public string? Estado { get; set; } = "A";

        public DateTime? Fec_registro { get; set; } = null;
    }
}
