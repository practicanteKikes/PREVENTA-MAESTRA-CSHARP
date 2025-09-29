using MainProject.Services.CustomSybase.Models.SnapModel;

namespace MainProject.Services.CustomSybase.Models.QueryCmLineasProductos.All
{
    public class FnResBdQueryCmLineasProductosAllAzync
    {
        public bool Ksuccess { get; set; } = false;
        public string Kmessage { get; set; } = "";
        public List<LineasProductosSnapModel> Kdata { get; set; } = new List<LineasProductosSnapModel>(); // Por defecto un array vacio
    }
}
