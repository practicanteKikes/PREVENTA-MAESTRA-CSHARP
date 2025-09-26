namespace MainProject.Services.CustomDatabaseOutput.Models.Json
{
    public class BancoConvenioOutputJsonModel
    {
        public int? Id { get; set; } = null;

        public string? Cod_banconal { get; set; } = null;
        public string? Descripcion { get; set; } = null;
        public DateTime? FecRegistro { get; set; } = null;
        public string? Cuenta_banco { get; set; } = null;
        public string? Cod_convenio_recaudo { get; set; } = null;
        public DateTime? Fecha { get; set; } = null;        
    }
}
