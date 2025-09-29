namespace MainProject.Controllers.CustomMunicipios.Models.MunicipiosAll.MainGetMunicipiosAllAsync.Json
{
    public class ModelMunicipioAllJsonCustom
    {
        // Original columns en la vista
        // [id,id_departamento,descripcion,fec_registro,fecha]


        public int? Id { get; set; } = 1;
        public string? Id_ciudad { get; set; } = null;
        public string? Id_departamento { get; set; } = null;
        public string? Nombre_ciudad { get; set; } = null;       
        public DateTime? Fec_registro { get; set; } = null;
        public DateTime? Fecha { get; set; } = null;
    }
}
