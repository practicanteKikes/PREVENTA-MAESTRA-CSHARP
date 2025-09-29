namespace MainProject.Services.CustomSybase.Models.QueryTiposNegocioFilteredTotalRowsAzync.FnRes
{
    public class FnResQueryTiposNegocioFilteredTotalRowsAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<int> Kdata { get; set; } = new List<int>(); // Por defecto un array vacio
    }
}
