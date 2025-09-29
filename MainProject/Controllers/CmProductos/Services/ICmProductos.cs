namespace MainProject.Controllers.CmProductos.Services
{
    public interface ICmProductos
    {
        Task<string> CmProductosMainGetAllAsync(string ls_ctrl_json);
        Task<string> CmProductosMainGetFilteredAsync(string ls_ctrl_json);
    }
}
