namespace MainProject.Controllers.CustomMunicipios.Models.MunicipiosFiltered.ObtainTotalRowsFilteredWithOutLimitMunicipios
{
    public class FnResObtainTotalRowsFWOLMunicipios
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<int> Kdata { get; set; } = new List<int>();
    }
}
