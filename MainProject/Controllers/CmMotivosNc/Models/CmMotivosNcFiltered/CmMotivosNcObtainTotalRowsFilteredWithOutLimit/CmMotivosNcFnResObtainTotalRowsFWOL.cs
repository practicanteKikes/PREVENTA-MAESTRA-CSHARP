namespace MainProject.Controllers.CmMotivosNc.Models.CmMotivosNcFiltered.CmMotivosNcObtainTotalRowsFilteredWithOutLimit
{
    public class CmMotivosNcFnResObtainTotalRowsFWOL
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public string KerrorCode { get; set; } = "0"; // [string] - Código de aceptación: 0, 1, 82, 83, 84
        public List<int> Kdata { get; set; } = new List<int>();
    }
}
