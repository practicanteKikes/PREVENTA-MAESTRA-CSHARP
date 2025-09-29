namespace MainProject.Controllers.CustomDepartamentos.Models.DepartamentosFiltered.ObtainDepartamentosObjectListAzync
{
    public class ResAjaxDepartamentosFilteredAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<ModelDepartamentoJsonFiltered> Kdata { get; set; } = new List<ModelDepartamentoJsonFiltered>();
    }
}
