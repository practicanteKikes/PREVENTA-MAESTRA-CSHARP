namespace MainProject.Services.CustomSybase.Models.QueryAAAModelDefaultFnResFTRowsAzync
{
    public class FnResQueryAAAModelDefaultFnResFTRowsAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<int> Kdata { get; set; } = new List<int>(); // Por defecto un array vacio
    }
}
