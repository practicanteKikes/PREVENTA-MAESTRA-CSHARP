namespace MainProject.Controllers.CustomDepartamentos.Models.DepartamentosFiltered.ObtainDepartamentosObjectListAzync
{
    public class FnResObtainDepartamentosObjList
    {
        // DEFAULT KIKES DEVELOPERS RESPONSES        
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";

        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<ModelDepartamentoJsonFiltered> Kdata { get; set; } = new List<ModelDepartamentoJsonFiltered>();
    }
}
