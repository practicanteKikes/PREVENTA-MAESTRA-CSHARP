namespace MainProject.Controllers.CustomDepartamentos.Models.DepartamentosAll.MainGetDepartamentosAllAsync.Json
{
    public class ModelDepartamentoAllJsonCustom
    {
        // Original columns en la vista
        // [id,id_departamento,descripcion,fec_registro,fecha]


        public int? Id { get; set; } = 1;
        public string? Id_departamento { get; set; } = null;
        public string? Descripcion { get; set; } = null;       
        public DateTime? Fec_registro { get; set; } = null;
        public DateTime? Fecha { get; set; } = null;
    }
}
