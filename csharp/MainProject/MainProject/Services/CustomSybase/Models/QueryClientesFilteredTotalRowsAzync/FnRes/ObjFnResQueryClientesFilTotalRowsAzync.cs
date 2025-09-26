namespace MainProject.Services.CustomSybase.Models.QueryClientesFilteredTotalRowsAzync.FnRes
{
    public class ObjFnResQueryClientesFilTotalRowsAzync
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public List<int> Data { get; set; } = new List<int>(); // Por defecto un array vacio
    }
}
